using System;
using System.Windows.Forms;

class ListaDupla<Dado> : IDados<Dado>
                where Dado : IComparable<Dado>, IRegistro<Dado>, new()
{
    NoDuplo<Dado> primeiro, ultimo, atual;
    int quantosNos;

    public ListaDupla()
    {
        primeiro = ultimo = atual = null;
        quantosNos = 0; 
    }

    public Situacao SituacaoAtual { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int PosicaoAtual { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool EstaNoInicio => throw new NotImplementedException();
    public bool EstaNoFim => throw new NotImplementedException();
    public bool EstaVazio => throw new NotImplementedException();          // (bool) Verificar se está vazia
    public int Tamanho => throw new NotImplementedException();

    public void LerDados(string nomeArquivo)    // fará a leitura e armazenamento dos dados do arquivo cujo nome é passado por parâmetro
    {
        throw new NotImplementedException();
    }
    public void GravarDados(string nomeArquivo)  // gravará sequencialmente, no arquivo cujo nome é passado por parâmetro, os dados armazenados na lista
    {
        throw new NotImplementedException();
    }
    public void PosicionarNoPrimeiro()        // Posicionar atual no primeiro nó para ser acessado
    {
        throw new NotImplementedException();
    }
    public void RetrocederPosicao()        // Retroceder atual para o nó anterior para ser acessado
    {
        throw new NotImplementedException();
    }
    public void AvancarPosicao()
    {
        throw new NotImplementedException();
    }
    public void PosicionarNoUltimo()        // posicionar atual no último nó para ser acessado
    {
        throw new NotImplementedException();
    }
    public void PosicionarEm(int posicaoDesejada)
    {
        throw new NotImplementedException();
    }

    // (bool) Pesquisar Dado procurado em ordem crescente; a pesquisa
    // posicionará o ponteiro atual no nó procurado quando este
    // or encontrado ou, se não achar, no nó seguinte a local
    // onde deveria estar o nó procurado
    public bool Existe(Dado procurado, out int ondeEsta)
    {
        throw new NotImplementedException();
    }
    public bool Excluir(Dado dadoAExcluir)
    {
        throw new NotImplementedException();
    }
    public bool IncluirNoInicio(Dado novoValor)
    {
        throw new NotImplementedException();
    }
    public bool IncluirAposFim(Dado novoValor)
    {
        throw new NotImplementedException();
    }
        public bool Incluir(Dado novoValor)         // (bool) Inserir nó com Dado em ordem crescente
    {
        throw new NotImplementedException();
    }
    public bool Incluir(Dado novoValor, int posicaoDeInclusao)  // inclui novo nó na posição indicada da lista
    {
        throw new NotImplementedException();
    }
    public Dado this[int indice]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    public Dado DadoAtual()  // retorna o dado atualmente visitado
    {
        throw new NotImplementedException();
    }
    public void ExibirDados()   // lista os dados armazenados na lista em modo console
    {
        throw new NotImplementedException();
    }
    public void ExibirDados(ListBox lista)  // lista os dados armazenados na lista no listbox passado como parâmetro
    {
        throw new NotImplementedException();
    }
    public void ExibirDados(ComboBox lista) // lista os dados armazenados na lista no combobox passado como parâmetro
    {
        throw new NotImplementedException();
    }
    public void ExibirDados(TextBox lista)
    {
        throw new NotImplementedException();
    }
    public void Ordenar()
    {
        throw new NotImplementedException();
    }
}