using Effizienze_Graphentheorie.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effizienze_Graphentheorie.Testumgebung
{
    public enum Algorithm { FordFulkerson, EdmondsKarp, Dinic, Preflow}
    class AlgoEvaluation
    {
        public Algorithm type;
        public bool isFeasible;
        public int flow;
        public Dictionary<Arc, int> capacity;
    }
}
