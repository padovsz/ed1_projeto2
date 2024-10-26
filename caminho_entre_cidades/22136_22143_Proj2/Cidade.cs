using System;
using System.IO;
using apArvore1;
using System.Windows.Forms;

// Nome: Hugo Gomes Soares - RA: 22136
// Nome: Maria Eduarda de Jesus Padovan - RA: 22143
class Cidade : IComparable<Cidade>, IRegistro, ICriterioDeSeparacao
  {
    const int tamNome = 15,
              tamX = 6,
              tamY = 6;

    const int iniNome = 0,
              iniX = iniNome + tamNome,
              iniY = iniX + tamX;

    const int tamanhoRegistro = tamNome +
                                sizeof(double) +
                                sizeof(double);
                                

    string nome;
    double x, y;

    ListaSimples<Ligacao> saidas;

    public string Nome   { get => nome; set => nome = value.PadRight(tamNome, ' ').Substring(0, tamNome); }
    public double X         { get => x; set => x = value; }
    public double Y         { get => y; set => y = value; }
    public ListaSimples<Ligacao> Saidas { get => saidas; set => saidas = value; }

    public int TamanhoRegistro { get => tamanhoRegistro; }

    public Cidade(string nome, double x, double y)
    {
         Nome = nome;
        X = x;
        Y = y;
        saidas = new ListaSimples<Ligacao>();
    }

    public Cidade()
    {
        Nome = "Nome";
        X = 0;
        Y = 0;
        Saidas = new ListaSimples<Ligacao>();
    }

    public int CompareTo(Cidade outro)
    {
        return nome.ToUpperInvariant().CompareTo(outro.nome.ToUpperInvariant());
    }

    public void LerRegistro(BinaryReader arquivo, long qualRegistro)
    {
      if (arquivo != null) // arquivo aberto?
      {
            try
            {
                long qtosBytes = qualRegistro * TamanhoRegistro;
                arquivo.BaseStream.Seek(qtosBytes, SeekOrigin.Begin);

                char[] umNome = new char[tamNome];
                umNome = arquivo.ReadChars(tamNome);
                string nomeLido = new string(umNome);
                this.Nome = nomeLido;
                this.X = arquivo.ReadDouble();
                this.Y = arquivo.ReadDouble();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
      }
    }

    public void GravarRegistro(BinaryWriter arq)
    {
      if (arq != null)  // arquivo de saída aberto?
      {
        char[] umNome = new char[tamNome];
            for (int i = 0; i < tamNome; i++)
                umNome[i] = this.Nome[i];
        arq.Write(umNome);
        arq.Write(X);
        arq.Write(Y);
      }
    }
    public string ParaArquivo()
    {
      return Nome + X.ToString().PadRight(tamX, '0') + Y.ToString().PadRight(tamY, '0');
    }

    public override string ToString()
    {
        return Nome + "\n" + Saidas.QuantosNos(); 
    }

    public bool PodeSeparar()
    {
        throw new NotImplementedException();
    }
}
