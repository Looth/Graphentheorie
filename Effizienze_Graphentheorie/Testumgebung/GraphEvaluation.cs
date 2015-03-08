using Effizienze_Graphentheorie.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effizienze_Graphentheorie.Testumgebung
{
    class GraphEvaluation
    {
        private DirectedGraph graph;
        private List<AlgoEvaluation> performances;


        public GraphEvaluation(DirectedGraph graph)
        {
            this.graph = graph;
            performances = new List<AlgoEvaluation>();

            performances.Add(EvaluateFordFulkerson());
            performances.Add(EvaluateEdmondsKarp());
            performances.Add(EvaluateDinic());
            performances.Add(EvaluatePreflow());
        }

        private AlgoEvaluation EvaluateFordFulkerson()
        {
            AlgoEvaluation ret = new AlgoEvaluation();
            graph.ResetGraph();

            DirectedGraph.AlgoOutput output = graph.FordFulkersonStep();

            while (output.UsedArcs.Length != 0)
            {
                output = graph.FordFulkersonStep();
            }

            ret.type = Algorithm.FordFulkerson;
            ret.flow = FlowOut();
            ret.isFeasible = CheckIfFeasible();
            ret.capacity = Capacities();

            return ret;
        }

        private AlgoEvaluation EvaluateEdmondsKarp()
        {
            AlgoEvaluation ret = new AlgoEvaluation();
            graph.ResetGraph();
            DirectedGraph.AlgoOutput output = graph.EdmondsKarpStep();

            while (output.UsedArcs.Length != 0)
            {
                output = graph.EdmondsKarpStep();
            }

            ret.type = Algorithm.EdmondsKarp;
            ret.flow = FlowOut();
            ret.isFeasible = CheckIfFeasible();
            ret.capacity = Capacities();

            return ret;
        }

        private AlgoEvaluation EvaluateDinic()
        {
            AlgoEvaluation ret = new AlgoEvaluation();
            graph.ResetGraph();
            DirectedGraph.AlgoOutput output = graph.DinicStep();

            while (output.UsedArcs.Length != 0)
            {
                output = graph.DinicStep();
            }

            ret.type = Algorithm.Dinic;
            ret.flow = FlowOut();
            ret.isFeasible = CheckIfFeasible();
            ret.capacity = Capacities();

            return ret;
        }

        private AlgoEvaluation EvaluatePreflow()
        {
            AlgoEvaluation ret = new AlgoEvaluation();
            graph.ResetGraph();

            DirectedGraph.PreflowOutput output = graph.PreflowStep();

            while (output.distanceIncreased != null || output.saturated != null)
            {
                output = graph.PreflowStep();
            }

            ret.type = Algorithm.Preflow;
            ret.flow = FlowOut();
            ret.isFeasible = CheckIfFeasible();
            ret.capacity = Capacities();

            return ret;
        }

        private bool CheckIfFeasible()
        {
            foreach (Node n in graph.Nodes)
            {
                if (!n.IsSource && !n.IsDrain)
                {
                    int throughput = 0;
                    foreach (Arc a in n.Outgoing)
                        throughput += a.Capacity;
                    foreach (Arc a in n.Incoming)
                        throughput -= a.Capacity;
                    if (throughput != 0)
                        return false;
                }
            }
            return true;
        }

        private int FlowOut()
        {
            int flow = 0;
            foreach (Arc a in graph.Source.Outgoing)
            {
                flow += a.Capacity;
            }
            return flow;
        }

        private Dictionary<Arc, int> Capacities()
        {
            Dictionary<Arc, int> capacity = new Dictionary<Arc, int>();
            foreach (Node n in graph.Nodes)
            {
                foreach (Arc a in n.Outgoing)
                {
                    capacity[a] = a.Capacity;
                }
            }
            return capacity;
        }

        public DirectedGraph Graph
        {
            get { return graph; }
        }
        public List<AlgoEvaluation> Performances
        {
            get { return performances; }
        }
    }
}
