using System;
using System.IO;

namespace apArvore1
{
  public interface IRegistro
  {
    void LerRegistro(BinaryReader arquivo, long qualRegistro);
    void GravarRegistro(BinaryWriter arquivo);
    int  TamanhoRegistro { get; }
  }
}
