using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

  interface IStack<Dado>
  {
    void Empilhar(Dado elemento);
    Dado Desempilhar();
    Dado OTopo();
    int Tamanho();
    bool EstaVazia();
  }
