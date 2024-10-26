using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

  class PilhaVetor<Dado> : IStack<Dado>
  {
    int maximoPosicoes;
    Dado[] p;   //  vetor onde serão armazenados os dados empilhados
    int topo;   //  índice da posição usada por último nesse vetor

    public PilhaVetor(int maximo)
    {
      p = new Dado[maximo];
      topo = -1;
      maximoPosicoes = maximo;  // guardo o número máximo de elementos do vetor p
    }
    public Dado Desempilhar()
    {
      if (EstaVazia())
        throw new PilhaVaziaException("Pilha esvaziou!");

      var valor = p[topo];
      topo--;
      return valor;
    }

    public void Empilhar(Dado elemento)
    {
      if (topo == maximoPosicoes)
        throw new Exception("Pilha transbordou!");

      topo++;
      p[topo] = elemento;
    }

    public bool EstaVazia()
    {
      return topo < 0;
    }

    public Dado OTopo()
    {
      if (EstaVazia())
        throw new PilhaVaziaException("Pilha esvaziou!");

      return p[topo];  // devolve o valor armazenado na última posição em uso do vetor p
    }

    public int Tamanho()
    {
      return topo+1;
    }

    public void Exibir(DataGridView dgv, Label lb, int largura)
    {
        dgv.RowCount = 2;
        dgv.ColumnCount = topo+1;
        for (int i=0; i <= topo; i++)
        {
            dgv.Columns[i].Width = largura;
            dgv[i, 0].Value = i + "";
            dgv[i, 1].Value = p[i];
        }
        lb.Text = topo + "";
        System.Threading.Thread.Sleep(200);
        Application.DoEvents();
    }

    public PilhaVetor<Dado> Clone()
    {
        var aux = new PilhaVetor<Dado>(1000);
        var retorno = new PilhaVetor<Dado>(1000);
        while(!this.EstaVazia())
        {
            aux.Empilhar(this.Desempilhar());
        }
        while(!aux.EstaVazia())
        {
            var dado = aux.Desempilhar();
            this.Empilhar(dado);
            retorno.Empilhar(dado);
        }

        return retorno;
    }
  }
