using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apGrafo
{
    class DistOriginal
    {
        public int distancia;
        public int tempo;
        public int verticePai;
        public DistOriginal(int vp, int d, int t)
        {
            distancia  = d;
            tempo = t;
            verticePai = vp;
        }
    }
}
