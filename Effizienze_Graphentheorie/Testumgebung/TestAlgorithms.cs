using Effizienze_Graphentheorie.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effizienze_Graphentheorie.Testumgebung
{
    class TestAlgorithms
    {

        public Dictionary<Tripel, List<GraphEvaluation>> tripelEnviroment = new Dictionary<Tripel, List<GraphEvaluation>>();


        public TestAlgorithms(List<Tripel> config)
        {
            RndGraphGenerator generator = new RndGraphGenerator(0, 500, 500);
            Random randomGenerator = new Random(1);

            foreach (Tripel t in config)
            {
                List<DirectedGraph> graphsInTripel = new List<DirectedGraph>();
                List<GraphEvaluation> evaluations = new List<GraphEvaluation>();

                for (int i = 0; i < t.instances; i++)
                {
                    DirectedGraph toAdd = generator.GenerateGraph(t.nodeCount, t.maxCapacity);
                    toAdd.SetSource(toAdd.Nodes[randomGenerator.Next(toAdd.Nodes.Count)]);
                    toAdd.SetDrain(toAdd.Nodes[randomGenerator.Next(toAdd.Nodes.Count)]);
                    graphsInTripel.Add(toAdd);
                }

                foreach (DirectedGraph g in graphsInTripel)
                {
                    GraphEvaluation eval = new GraphEvaluation(g);
                    evaluations.Add(eval);

                }
                tripelEnviroment[t] = evaluations;
            }


        }

        public bool CheckIfAllFeasible()
        {
            foreach (List<GraphEvaluation> evaluations in tripelEnviroment.Values)
            {
                foreach (GraphEvaluation g_eval in evaluations)
                {
                    foreach (AlgoEvaluation a_eval in g_eval.Performances)
                        if (!a_eval.isFeasible)
                            return false;
                }
            }
            return true;
        }

        public bool CheckIfFlowIsIdentical()
        {
            foreach (List<GraphEvaluation> evaluations in tripelEnviroment.Values)
            {
                foreach (GraphEvaluation g_eval in evaluations)
                {
                    int flow = g_eval.Performances[0].flow;
                    foreach (AlgoEvaluation a_eval in g_eval.Performances)
                        if (flow != a_eval.flow)
                            return false;
                }
            }
            return true;
        }
    }
}
