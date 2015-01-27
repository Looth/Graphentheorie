using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effizienze_Graphentheorie.Graph
{
    class RndGraphGenerator
    {
        private int circleSize;
        private int canvasHeight;
        private int canvasWidth;

        public RndGraphGenerator(int circleSize, int canvasHeight, int canvasWidth)
        {
            this.circleSize = circleSize;
            this.canvasHeight = canvasHeight;
            this.canvasWidth = canvasWidth;
        }

        public DirectedGraph GenerateGraph(int nodeCount, int maxCapacity, int circleSize)
        {
            DirectedGraph result = new DirectedGraph();
            Node[] createdNodes = new Node[nodeCount];
            List<Triple> distances = new List<Triple>();
            int nodesCreated = 0;
            Random random = new Random();

            //create N random nodes
            while (nodesCreated < nodeCount)
            {
                int xpos = (int) (circleSize / 2.0 + random.NextDouble() * (canvasWidth - circleSize));
                int ypos = (int) (circleSize / 2.0 + random.NextDouble() * (canvasHeight - circleSize));
                Node node = new Node(xpos, ypos, nodesCreated.ToString(), circleSize);
                bool passed = true;
                List<Triple> distanceToNewNode = new List<Triple>();
                for (int i = 0; i < nodesCreated; i++)
                {
                    double distance = node.DistanceTo(createdNodes[i]);
                    if (distance < circleSize + 8)
                    {
                        passed = false;
                        break;
                    }
                    Triple distanceEntry = new Triple();
                    distanceEntry.n1 = node;
                    distanceEntry.n2 = createdNodes[i];
                    distanceEntry.distance = distance;
                    distanceToNewNode.Add(distanceEntry);
                }
                if (passed)
                {
                    distances.AddRange(distanceToNewNode);
                    createdNodes[nodesCreated] = node;
                    nodesCreated++;
                }
            }
            //create Arcs

            distances.Sort(delegate(Triple x, Triple y)
            {
                return (int) (x.distance - y.distance);
            });


            for (int i = 0; i < distances.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (LinesIntersect(distances[i], distances[j]))
                    {
                        distances.RemoveAt(i);
                        i--;
                        break;
                    }

                }
            }


            foreach (Node n in createdNodes)
                result.AddNode(n);

            foreach (Triple t in distances)
            {
                Arc way = new Arc(t.n1, t.n2, random.Next(maxCapacity));
                Arc reverseWay = new Arc(t.n2, t.n1, random.Next(maxCapacity));
                way.ReverseArc = reverseWay;
                reverseWay.ReverseArc = way;
                result.AddArc(way);
                result.AddArc(reverseWay);
            }

            return result;
        }

        private bool LinesIntersect(Triple t1, Triple t2)
        {
            //if 2 Lines start or end at the same point, it doesnt count as intersection
            if (t1.n1 == t2.n1 || t1.n2 == t2.n1 || t1.n2 == t2.n2 || t1.n1 == t2.n2) return false;

            /*
            A = y2-y1
            B = x1-x2
            C = A*x1+B*y1
            */
            double x11 = t1.n1.XPos;
            double x12 = t1.n2.XPos;
            double y11 = t1.n1.YPos;
            double y12 = t1.n2.YPos;

            double x21 = t2.n1.XPos;
            double x22 = t2.n2.XPos;
            double y21 = t2.n1.YPos;
            double y22 = t2.n2.YPos;

            double A1 = y12 - y11;
            double B1 = x11 - x12;
            double C1 = A1 * x11 + B1 * y11;

            double A2 = y22 - y21;
            double B2 = x21 - x22;
            double C2 = A2 * x21 + B2 * y21;

            double det = A1 * B2 - A2 * B1;

            if (det == 0) return false;
            
            //intersection point
            double x = (B2 * C1 - B1 * C2) / det;
            double y = (A1 * C2 - A2 * C1) / det;

            // min(x1,x2) ≤ x ≤ max(x1,x2)
            return Math.Min(x11, x12) <= x
                && x <= Math.Max(x11, x12)
                && Math.Min(y11, y12) <= y
                && y <= Math.Max(y11, y12)
                && Math.Min(x21, x22) <= x
                && x <= Math.Max(x21, x22)
                && Math.Min(y21, y22) <= y
                && y <= Math.Max(y21, y22);
        } 

        struct Triple
        {
            public Node n1;
            public Node n2;
            public double distance;
        }
    }
}
