using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

  class PilhaVaziaException : Exception
  {
    public PilhaVaziaException(string mensagem) : base(mensagem)
    {
    }
  }