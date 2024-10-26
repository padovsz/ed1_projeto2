using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using apArvore1;
using apGrafo;

namespace _22136_22143_Proj1ED
{
    // Nome: Hugo Gomes Soares - RA: 22136
    // Nome: Maria Eduarda de Jesus Padovan - RA: 22143

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public enum Situacao
        {
            navegando, excluindo, incluindo, pesquisando, editando
        }

        Grafo oGrafo;
        Arvore<Cidade> arvoreCidades;
        Situacao situacaoAtual;

        // quando o formulário for carregado, será feita a leitura
        // dos arquivos necessários para o programa funcionar
        private void Form1_Load(object sender, EventArgs e)
        {
            if (dlgCidades.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    arvoreCidades = new Arvore<Cidade>();
                    arvoreCidades.LerArquivoDeRegistros(dlgCidades.FileName);
                    arvoreCidades.PosicionarNoPrimeiro();

                    pbMapa.Invalidate();

                    situacaoAtual = Situacao.navegando;


                    if (dlgCaminhos.ShowDialog() == DialogResult.OK)
                    {
                        var origem = new FileStream(dlgCaminhos.FileName, FileMode.OpenOrCreate);
                        var arquivo = new BinaryReader(origem);
                        Ligacao novaLigacao = new Ligacao();
                        int posicaoFinal = (int)arquivo.BaseStream.Length / novaLigacao.TamanhoRegistro - 1;
                        arquivo.BaseStream.Seek(0, SeekOrigin.Begin);
                        for (int i = 0; i <= posicaoFinal; i++)
                        {
                            novaLigacao = new Ligacao();
                            novaLigacao.LerRegistro(arquivo, i);
                            var cidadeProcurada = new Cidade(novaLigacao.IdCidadeOrigem, 0, 0);
                            if (arvoreCidades.Existe(cidadeProcurada))
                                arvoreCidades.Atual.Info.Saidas.InserirEmOrdem(novaLigacao);
                        }
                        arvoreCidades.PosicionarNoPrimeiro();
                        arquivo.Close();
                    }

                    // grafo caminhos
                    oGrafo = new Grafo(null);
                    PercorrerInOrdem(arvoreCidades.Raiz, (NoArvore<Cidade> r) =>
                    {
                        oGrafo.NovoVertice(r.Info.Nome);
                        cbOrigem.Items.Add(r.Info.Nome);
                        cbDestino.Items.Add(r.Info.Nome);
                    });

                    PercorrerInOrdem(arvoreCidades.Raiz, (NoArvore<Cidade> r) =>
                    {
                        r.Info.Saidas.IniciarPercursoSequencial();
                        while (r.Info.Saidas.PodePercorrer())
                        {
                            oGrafo.NovaAresta(cbOrigem.Items.IndexOf(r.Info.Saidas.Atual.Info.IdCidadeOrigem),
                                              cbDestino.Items.IndexOf(r.Info.Saidas.Atual.Info.IdCidadeDestino),
                                              r.Info.Saidas.Atual.Info);
                            oGrafo.NovaAresta(cbDestino.Items.IndexOf(r.Info.Saidas.Atual.Info.IdCidadeDestino),
                                                cbOrigem.Items.IndexOf(r.Info.Saidas.Atual.Info.IdCidadeOrigem),
                                                r.Info.Saidas.Atual.Info);
                        }
                    });

                    AtualizarTela();
                }
                catch (Exception)
                {
                    MessageBox.Show("Erro de leitura no arquivo.");
                }
            }
        }

        // no picturebox, assim que o arquivo solicitado for aberto
        // serão desenhadas as cidades descritas no mesmo (sobrescrevendo
        // a imagem do mapa)
        private void pbMapa_Paint(object sender, PaintEventArgs e)
        {
            if (arvoreCidades != null)
            {
                Graphics g = e.Graphics;
                Pen pen = new Pen(Color.Coral, 3);
                Brush brush = new SolidBrush(Color.LightGray);
                Brush brush2 = new SolidBrush(Color.Black);
                Font fonte = new Font("Arial", 10);

                pen.Color = Color.Black;
                pen.Width = 2;

                PercorrerInOrdem(arvoreCidades.Raiz, (NoArvore<Cidade> r) =>
                {
                    Cidade cidade = r.Info;

                    // DESENHAR CIDADES
                    if (cidade == arvoreCidades.Atual.Info)
                    {
                        Pen penAtual = new Pen(Color.Black, 2);
                        Brush brushAtual = new SolidBrush(Color.Yellow);
                        g.DrawEllipse(penAtual, (float)cidade.X * pbMapa.Width - 5, (float)cidade.Y * pbMapa.Height - 5, 10, 10);
                        g.FillEllipse(brushAtual, (float)cidade.X * pbMapa.Width - 4, (float)cidade.Y * pbMapa.Height - 4, 8, 8);
                        g.DrawString(cidade.Nome, fonte, brush2, (float)cidade.X * pbMapa.Width - 8, (float)cidade.Y * pbMapa.Height + 6);
                    }
                    else
                    {
                        g.DrawEllipse(pen, (float)cidade.X * pbMapa.Width - 5, (float)cidade.Y * pbMapa.Height - 5, 10, 10);
                        g.FillEllipse(brush, (float)cidade.X * pbMapa.Width - 4, (float)cidade.Y * pbMapa.Height - 4, 8, 8);
                        g.DrawString(cidade.Nome, fonte, brush2, (float)cidade.X * pbMapa.Width - 8, (float)cidade.Y * pbMapa.Height + 6);
                    }
                });
            }
        }

        void PercorrerInOrdem(NoArvore<Cidade> r, Action<NoArvore<Cidade>> operacao)
        {
            if (r != null)
            {
                PercorrerInOrdem(r.Esq, operacao);
                operacao(r);
                PercorrerInOrdem(r.Dir, operacao);
            }
        }

        // atualiza a tela de acordo com a situação em que o usuário está definindo como os botões
        // ficarão (por meio do método TestarBotoes, a mensagem que aparecerá no statusStrip para
        // auxiliar o usuário e os campos que precisarão ser limpos para que o usuário digite
        void AtualizarTela()
        {
            switch (situacaoAtual)
            {
                case Situacao.navegando:
                    {
                        Cidade cidadeAtual = arvoreCidades.Atual.Info;
                        TestarBotoes();

                        if (cidadeAtual != null)
                        {
                            txtNome.Text = cidadeAtual.Nome;
                            nudX.Value = (decimal)cidadeAtual.X;
                            nudY.Value = (decimal)cidadeAtual.Y;

                            //Colocar informaçoes no DGV
                            dgvCaminhos.Columns.Clear();
                            dgvCaminhos.Rows.Clear();
                            dgvCaminhos.Columns.Add("cabecalho", "");
                            dgvCaminhos.Rows.Add(2);
                            dgvCaminhos[0, 0].Value = "Destino  :";
                            dgvCaminhos[0, 1].Value = "Distancia:";
                            dgvCaminhos[0, 2].Value = "Tempo    :";

                            var saidasAtual = cidadeAtual.Saidas;

                            if (saidasAtual.Atual != null)
                            {
                                txtOrigem.Text = saidasAtual.Atual.Info.IdCidadeOrigem;
                                txtDestino.Text = saidasAtual.Atual.Info.IdCidadeDestino;
                                nudDistancia.Value = saidasAtual.Atual.Info.Distancia;
                                nudTempo.Value = saidasAtual.Atual.Info.Tempo;
                            }
                            else
                            {
                                if (!saidasAtual.EstaVazia)
                                {
                                    txtOrigem.Text = saidasAtual.Primeiro.Info.IdCidadeOrigem;
                                    txtDestino.Text = saidasAtual.Primeiro.Info.IdCidadeDestino;
                                    nudDistancia.Value = saidasAtual.Primeiro.Info.Distancia;
                                    nudTempo.Value = saidasAtual.Primeiro.Info.Tempo;
                                }
                                else
                                {
                                    txtOrigem.Text = "";
                                    txtDestino.Text = "";
                                    nudDistancia.Value = 0;
                                    nudTempo.Value = 0;
                                }
                            }

                            var posicaoAtual = saidasAtual.PosicaoAtual;

                            saidasAtual.IniciarPercursoSequencial();
                            int n = 1;
                            while (saidasAtual.PodePercorrer())
                            {
                                dgvCaminhos.Columns.Add($"{n}", "");
                                dgvCaminhos[n, 0].Value = saidasAtual.Atual.Info.IdCidadeDestino;
                                dgvCaminhos[n, 1].Value = saidasAtual.Atual.Info.Distancia + " Km";
                                dgvCaminhos[n, 2].Value = saidasAtual.Atual.Info.Tempo + " min";
                                n++;
                            }
                            saidasAtual.PosicaoAtual = posicaoAtual;
                        }
                        stRegistro.Items[0].Text = "Mensagem: Leia aqui as instruções para cada funcionalidade";
                        pbMapa.Invalidate();
                    }
                    break;

                case Situacao.pesquisando:
                    {
                        TestarBotoes();

                        txtNome.Text = "";
                        nudX.Value = 0;
                        nudY.Value = 0;

                        tcCaminhosCidades.SelectedTab = tpCidades;
                        txtNome.Focus();
                        stRegistro.Items[0].Text = "Mensagem: Digite o nome da cidade que deseja procurar e depois clique em salvar";
                    }
                    break;

                case Situacao.incluindo:
                    {
                        TestarBotoes();

                        txtNome.Text = "";
                        nudX.Value = 0;
                        nudY.Value = 0;

                        txtOrigem.Text = arvoreCidades.Atual.Info.Nome;
                        txtDestino.Text = "";
                        nudDistancia.Value = 0;
                        nudTempo.Value = 0;

                        if (tcCaminhosCidades.SelectedTab == tpCidades)
                        {
                            txtNome.Focus();
                            stRegistro.Items[0].Text = "Mensagem: Informe o nome da cidade, clique na posição do mapa onde deseja incluir e depois clique em salvar";
                        }
                        else
                        {
                            txtOrigem.ReadOnly = true;
                            txtDestino.Focus();
                            stRegistro.Items[0].Text = "Mensagem: Informe o nome do destino, a distância e o tempo do caminho a incluir e depois clique em salvar";
                        }
                    }
                    break;

                case Situacao.editando:
                    {
                        TestarBotoes();

                        var saidasAtual = arvoreCidades.Atual.Info.Saidas;

                        if (saidasAtual.Atual != null)
                        {
                            txtOrigem.Text = saidasAtual.Atual.Info.IdCidadeOrigem;
                            txtDestino.Text = saidasAtual.Atual.Info.IdCidadeDestino;
                            nudDistancia.Value = saidasAtual.Atual.Info.Distancia;
                            nudTempo.Value = saidasAtual.Atual.Info.Tempo;
                        }
                        else
                        {
                            txtDestino.Text = "";
                            nudDistancia.Value = 0;
                            nudTempo.Value = 0;

                        }

                        if (tcCaminhosCidades.SelectedTab == tpCidades)
                        {
                            txtNome.Focus();
                            stRegistro.Items[0].Text = "Mensagem: Digite o novo nome da cidade e clique no mapa onde deseja posicioná-la";
                        }
                        else
                        {
                            txtDestino.Focus();
                            stRegistro.Items[0].Text = "Mensagem: Selecione um caminho na lista de caminhos para editar e então clique em salvar";
                        }
                    }
                    break;
            }
        }

        // indica quais botões poderão ser habilitados de acordo
        // coma situação em que o usuário se encontra no programa
        void TestarBotoes()
        {
            switch (situacaoAtual)
            {
                case Situacao.navegando:
                    {
                        btnProcurar.Enabled = true;
                        btnNovo.Enabled = true;
                        btnSalvar.Enabled = true;
                        btnExcluir.Enabled = true;
                        btnEditar.Enabled = true;
                        tpCaminhos.Enabled = true;
                        tpCidades.Enabled = true;
                        txtOrigem.ReadOnly = false;
                    }
                    break;

                case Situacao.pesquisando:
                    {
                        btnNovo.Enabled = false;
                        btnExcluir.Enabled = false;
                        btnEditar.Enabled = false;
                        tpCaminhos.Enabled = false;
                    }
                    break;

                case Situacao.incluindo:
                    {
                        btnProcurar.Enabled = false;
                        btnExcluir.Enabled = false;
                        btnEditar.Enabled = false;

                        if (tcCaminhosCidades.SelectedTab == tpCaminhos)
                        {
                            tpCidades.Enabled = false;
                            txtOrigem.ReadOnly = true;
                        }
                        else
                        {
                            tpCaminhos.Enabled = false;
                        }
                    }
                    break;

                case Situacao.editando:
                    {
                        btnProcurar.Enabled = false;
                        btnExcluir.Enabled = false;
                        btnNovo.Enabled = false;

                        if (tcCaminhosCidades.SelectedTab == tpCaminhos)
                        {
                            tpCidades.Enabled = false;
                            txtOrigem.ReadOnly = true;
                            if (arvoreCidades.Atual.Info.Saidas.Atual == null)
                                btnSalvar.Enabled = false;
                            else
                                btnSalvar.Enabled = true;
                        }
                        else
                        {
                            tpCaminhos.Enabled = false;
                        }
                    }
                    break;
            }
        }

        // a partir daqui, o que será apresentado no código é o que cada
        // botão faz ao ser clicado pelo usuário, cada um realizando as suas
        // respectivas funções

        private void btnProcurar_Click(object sender, EventArgs e)
        {
            situacaoAtual = Situacao.pesquisando;
            AtualizarTela();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            switch (situacaoAtual)
            {
                case Situacao.navegando:
                    {
                        // chama o método que executa a operação de salvar em arquivo .txt
                        Salvar();
                    }
                    break;

                case Situacao.pesquisando:
                    {
                        // se a situação for pesquisando, será preciso ver se aquela cidade existe
                        // para que quando o usuário clique no botão de salvar, as respectivas informações
                        // daquela cidade apareçam nos devidos campos
                        if (arvoreCidades.Existe(new Cidade(txtNome.Text, 0, 0)))
                        {
                            situacaoAtual = Situacao.navegando;
                            AtualizarTela();
                        }
                        else
                        {
                            MessageBox.Show("A cidade procurada não está registrada!");
                        }
                    }
                    break;

                case Situacao.incluindo:
                    {
                        // se a situação for de inclusão, incluirei uma nova cidade com as informações digitadas 
                        // nos devidos campos pelo usuário
                        try
                        {
                            if (tcCaminhosCidades.SelectedTab == tpCidades)
                            {
                                if (txtNome.Text != "")
                                {
                                    arvoreCidades.IncluirNovoRegistro(new Cidade(txtNome.Text, (double)Math.Round(nudX.Value, 3), (double)Math.Round(nudY.Value, 3)));
                                    oGrafo.NovoVertice(txtNome.Text.PadRight(15, ' ').Substring(0, 15));
                                    cbOrigem.Items.Add(txtNome.Text.PadRight(15, ' ').Substring(0, 15));
                                    cbDestino.Items.Add(txtNome.Text.PadRight(15, ' ').Substring(0, 15));
                                    situacaoAtual = Situacao.navegando;
                                    AtualizarTela();
                                    pbMapa.Invalidate();
                                }
                                else
                                    MessageBox.Show("O nome da cidade não pode ser vazio");
                            }
                            else
                            {
                                var cidadeAtual = arvoreCidades.Atual.Info;
                                if (arvoreCidades.Existe(new Cidade(txtOrigem.Text, 0, 0)) && arvoreCidades.Existe(new Cidade(txtDestino.Text, 0, 0)))
                                {
                                    var ligacao = new Ligacao(txtDestino.Text, txtOrigem.Text, (int)Math.Round(nudDistancia.Value), (int)Math.Round(nudTempo.Value));
                                    oGrafo.NovaAresta(cbDestino.Items.IndexOf(txtDestino.Text.PadRight(15, ' ').Substring(0, 15)), cbOrigem.Items.IndexOf(txtOrigem.Text), ligacao);
                                    arvoreCidades.Existe(cidadeAtual);
                                    var ligacaoVolta = new Ligacao(txtOrigem.Text, txtDestino.Text, (int)Math.Round(nudDistancia.Value), (int)Math.Round(nudTempo.Value));
                                    arvoreCidades.Atual.Info.Saidas.InserirEmOrdem(ligacaoVolta);
                                    oGrafo.NovaAresta(cbOrigem.Items.IndexOf(txtOrigem.Text), cbDestino.Items.IndexOf(txtDestino.Text.PadRight(15, ' ').Substring(0, 15)), ligacaoVolta);
                                    situacaoAtual = Situacao.navegando;
                                    AtualizarTela();
                                }
                                else
                                {
                                    MessageBox.Show("Os nomes fornecidos são inválidos");
                                    arvoreCidades.Existe(cidadeAtual);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("A cidade que deseja incluir já está registrada!");
                        }
                    }
                    break;

                case Situacao.editando:
                    {
                        try
                        {
                            var cidade = arvoreCidades.Atual.Info;

                            if (tcCaminhosCidades.SelectedTab == tpCidades)
                            {
                                // não deve existir previamente
                                if (arvoreCidades.Existe(new Cidade(txtNome.Text, 0, 0)))
                                {
                                    arvoreCidades.Existe(cidade);
                                    throw new Exception("Cidade já existe");
                                }

                                arvoreCidades.Existe(cidade);
                                int iCidade = cbOrigem.Items.IndexOf(cidade.Nome);
                                cidade.Nome = txtNome.Text;
                                cidade.X = (double)Math.Round(nudX.Value, 3);
                                cidade.Y = (double)Math.Round(nudY.Value, 3);
                                oGrafo.EditarVertice(iCidade, cidade.Nome);
                                cbOrigem.Items[iCidade] = cidade.Nome;
                                cbDestino.Items[iCidade] = cidade.Nome;
                                situacaoAtual = Situacao.navegando;
                                AtualizarTela();
                                pbMapa.Invalidate();
                            }
                            else
                            {
                                if (cidade.Saidas.Atual != null)
                                {
                                    // deve existir previamente
                                    if (arvoreCidades.Existe(new Cidade(txtDestino.Text, 0, 0)))
                                    {
                                        arvoreCidades.Existe(cidade);
                                        int iOrigemOriginal = cbOrigem.Items.IndexOf(cidade.Saidas.Atual.Info.IdCidadeOrigem);
                                        int iDestinoOriginal = cbDestino.Items.IndexOf(cidade.Saidas.Atual.Info.IdCidadeDestino);
                                        cidade.Saidas.Atual.Info.IdCidadeOrigem = txtOrigem.Text;
                                        cidade.Saidas.Atual.Info.IdCidadeDestino = txtDestino.Text;
                                        cidade.Saidas.Atual.Info.Distancia = (int)Math.Round(nudDistancia.Value);
                                        cidade.Saidas.Atual.Info.Tempo = (int)Math.Round(nudTempo.Value);
                                        oGrafo.RemoverAresta(iDestinoOriginal, iOrigemOriginal);
                                        oGrafo.EditarAresta(iOrigemOriginal, iDestinoOriginal, 
                                                            cbDestino.Items.IndexOf(cidade.Saidas.Atual.Info.IdCidadeDestino), cidade.Saidas.Atual.Info);
                                        oGrafo.NovaAresta(cbDestino.Items.IndexOf(cidade.Saidas.Atual.Info.IdCidadeDestino), iOrigemOriginal, cidade.Saidas.Atual.Info);
                                    }
                                    else
                                    {
                                        MessageBox.Show("A cidade de destino precisa estar previamente cadastrada!");
                                        arvoreCidades.Existe(cidade);
                                    }

                                    situacaoAtual = Situacao.navegando;
                                    AtualizarTela();
                                }
                                else
                                    MessageBox.Show("É necessário selecionar um caminho a ser editado na lista de caminhos");
                            }

                        }
                        catch (Exception)
                        {
                            MessageBox.Show("O nome que deseja alterar já está em uso!");
                        }
                    }
                    break;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            situacaoAtual = Situacao.navegando;
            AtualizarTela();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            situacaoAtual = Situacao.incluindo;
            AtualizarTela();
        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            situacaoAtual = Situacao.editando;
            AtualizarTela();
        }

        private void pbMapa_MouseClick(object sender, MouseEventArgs e)
        {
            if (situacaoAtual == Situacao.incluindo || situacaoAtual == Situacao.editando)
            {
                nudX.Value = (decimal)e.X / pbMapa.Width;
                nudY.Value = (decimal)e.Y / pbMapa.Height;
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (tcCaminhosCidades.SelectedTab == tpCidades)
            {
                if (MessageBox.Show("Você deseja excluir essa cidade e todos os seus caminhos permanentemente?",
                                    "Deseja Excluir?",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    situacaoAtual = Situacao.excluindo;
                    int iCidade = cbOrigem.Items.IndexOf(arvoreCidades.Atual.Info.Nome);
                    oGrafo.RemoverVertice(iCidade);
                    arvoreCidades.ApagarNo(arvoreCidades.Atual.Info);
                    cbOrigem.Items.RemoveAt(iCidade);
                    cbDestino.Items.RemoveAt(iCidade);
                    situacaoAtual = Situacao.navegando;
                    AtualizarTela();
                    pbMapa.Invalidate();
                }
            }
            else
            {
                if (arvoreCidades.Atual.Info.Saidas.Atual != null)
                {
                    if (MessageBox.Show("Você deseja excluir esse caminho permanentemente?",
                                        "Deseja Excluir?",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        situacaoAtual = Situacao.excluindo;
                        int iOrig = cbOrigem.Items.IndexOf(arvoreCidades.Atual.Info.Nome);
                        int iDest = cbDestino.Items.IndexOf(arvoreCidades.Atual.Info.Saidas.Atual.Info.IdCidadeDestino);
                        oGrafo.RemoverAresta(iOrig, iDest);
                        oGrafo.RemoverAresta(iDest, iOrig);
                        arvoreCidades.Atual.Info.Saidas.Remover(arvoreCidades.Atual.Info.Saidas.Atual.Info);
                        situacaoAtual = Situacao.navegando;
                        AtualizarTela();
                    }
                }
                else
                    MessageBox.Show("É necessário antes selecionar um caminho na lista de caminhos para excluí-lo");
            }
        }

        // não permitir que o usuário saia do programa sem salvar as alterações que
        // o mesmo fez em seu arquivo durante a utilização do programa
        private void btnSair_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Você deseja realmente encerrar a execução do programa?",
                                "Deseja sair?",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Você deseja salvar antes de sair?",
                                "Deseja salvar?",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Salvar();
            }
        }

        void Salvar()
        {
            // gravará os dados armazenados na arvore no arquivo fornecido pelo usuário 
            arvoreCidades.GravarArquivoDeRegistros(dlgCidades.FileName);

            var destino = new FileStream(dlgCaminhos.FileName, FileMode.Create);
            var arquivo = new BinaryWriter(destino);
            PercorrerInOrdem(arvoreCidades.Raiz, (NoArvore<Cidade> no) =>
            {
                no.Info.Saidas.GravarRegistros(arquivo);
            });
            arquivo.Close();

            MessageBox.Show("Registros salvos!");
        }

        // tratamento de evento para exibição de caminho selecionado
        private void dgvCaminhos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                var cidadeAtual = arvoreCidades.Atual.Info;
                cidadeAtual.Saidas.ExisteDado(new Ligacao(cidadeAtual.Nome, dgvCaminhos[e.ColumnIndex, 0].Value.ToString(), 0, 0));
                AtualizarTela();
            }
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            pbArvore.Invalidate();
        }

        private void pbArvore_Paint(object sender, PaintEventArgs e)
        {
            arvoreCidades.DesenharArvore(pbArvore.Width / 2, 0, e.Graphics);
        }

        private void cbOrigem_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*arvoreCidades.Existe(new Cidade(cbOrigem.SelectedItem.ToString(), 0, 0));
            cbDestino.Enabled = true;
            cbDestino.Items.Clear();
            arvoreCidades.Atual.Info.Saidas.IniciarPercursoSequencial();
            while(arvoreCidades.Atual.Info.Saidas.PodePercorrer())
            {
                cbDestino.Items.Add(arvoreCidades.Atual.Info.Saidas.Atual.Info.IdCidadeDestino);
            }*/
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            lsbPercurso.Items.Clear();
            nudDistMin.Value = 0;
            nudTempoMin.Value = 0;
            lsbPercurso.Items.Add(oGrafo.Caminho(cbOrigem.SelectedIndex, cbOrigem.Items.IndexOf(cbDestino.SelectedItem.ToString()), lsbPercurso, nudDistMin, nudTempoMin));
        }
    }
}