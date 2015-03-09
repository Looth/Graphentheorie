using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effizienze_Graphentheorie.DataExport
{
    class JsonPerformance
    {
        public bool isFordFeasible;
        public bool isEdmondFeasible;
        public bool isDinicFeasible;
        public bool isPreflowFeasible;
        public int FordFlow;
        public int EdmondFlow;
        public int DinicFlow;
        public int PreflowFlow;
        public JsonGraph graph;
    }
}
