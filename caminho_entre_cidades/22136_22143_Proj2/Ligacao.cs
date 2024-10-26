using System;
using System.IO;
using apArvore1;

// Nome: Hugo Gomes Soares - RA: 22136
// Nome: Maria Eduarda de Jesus Padovan - RA: 22143
internal class Ligacao : IComparable<Ligacao>, IRegistro, ICriterioDeSeparacao
{
    const int tamCodigo = 15,
          tamDistancia = 4,
          tamTempo = 4;

    const int iniCodigoOrigem = 0,
              iniCodigoDestino = iniCodigoOrigem + tamCodigo,
              iniDistancia = iniCodigoDestino + tamCodigo,
              iniTempo = iniDistancia + tamDistancia;


    string idCidadeOrigem, idCidadeDestino;
    int distancia, tempo;

    const int tamanhoRegistro = tamCodigo + tamCodigo + sizeof(int) + sizeof(int);
    

    public Ligacao(string idCidadeOrigem, string idCidadeDestino, int distancia, int tempo)
    {
        this.IdCidadeOrigem = idCidadeOrigem;
        this.IdCidadeDestino = idCidadeDestino;
        this.Distancia = distancia;
        this.Tempo = tempo;
    }

    public Ligacao()
    {
        this.idCidadeDestino = "";
        this.idCidadeOrigem = "";
        this.tempo = 0;
        this.distancia = 0;
    }

    public string IdCidadeOrigem { get => idCidadeOrigem; set => idCidadeOrigem = value.PadRight(tamCodigo, ' ').Substring(0, tamCodigo); }
    public string IdCidadeDestino { get => idCidadeDestino; set => idCidadeDestino = value.PadRight(tamCodigo, ' ').Substring(0, tamCodigo); }
    public int Distancia { get => distancia; set => distancia = value; }
    public int Tempo { get => tempo; set => tempo = value; }

    public int TamanhoRegistro { get => tamanhoRegistro; }

    public int CompareTo(Ligacao outro)
    {
        return (idCidadeOrigem.ToUpperInvariant() + idCidadeDestino.ToUpperInvariant()).CompareTo(
                outro.idCidadeOrigem.ToUpperInvariant() + outro.idCidadeDestino.ToUpperInvariant());
    }

    public void LerRegistro(BinaryReader arquivo, long qualRegistro)
    {
        if (arquivo != null) // arquivo aberto?
        {
            long qtsBytes = qualRegistro * TamanhoRegistro;
            arquivo.BaseStream.Seek(qtsBytes, SeekOrigin.Begin);
            IdCidadeOrigem = new string (arquivo.ReadChars(tamCodigo));
            IdCidadeDestino = new string (arquivo.ReadChars(tamCodigo));
            Distancia = arquivo.ReadInt32();
            Tempo = arquivo.ReadInt32();
        }
    }
    public void GravarRegistro(BinaryWriter arq)
    {
        if (arq != null)  // arquivo de saída aberto?
        {
            char[] codOrigem = new char[tamCodigo];
            for (int i = 0; i < tamCodigo; i++)
                codOrigem[i] = this.IdCidadeOrigem[i];
            arq.Write(codOrigem);

            char[] codDestino = new char[tamCodigo];
            for (int i = 0; i < tamCodigo; i++)
                codDestino[i] = this.IdCidadeDestino[i];
            arq.Write(codDestino);

            arq.Write(this.Distancia);
            arq.Write(this.Tempo);
        }
    }

    public string ParaArquivo()
    {
        return $"{IdCidadeOrigem}{IdCidadeDestino}{Distancia:00000}{Tempo:0000}";
    }

    public override string ToString()
    {
        return $"{IdCidadeOrigem} {IdCidadeDestino} {Distancia:00000} {Tempo:0000} ";
    }

    public bool PodeSeparar()
    {
        throw new NotImplementedException();
    }
}