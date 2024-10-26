using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apGrafo
{
  class Vertice
  {
    string rotulo;
    bool foiVisitado;
    bool estaAtivo;

    public string Rotulo { get => rotulo; set => rotulo = value.ToUpperInvariant(); }
    public bool FoiVisitado { get => foiVisitado; set => foiVisitado = value; }
    public bool EstaAtivo { get => estaAtivo; set => estaAtivo = value; }

    public Vertice(string label)
    {
      Rotulo = label;
      FoiVisitado = false;
      EstaAtivo = true;
    }
  }
}
