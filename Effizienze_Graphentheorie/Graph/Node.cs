using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Effizienze_Graphentheorie.Graph
{
    class Node
    {
        private int xPos;
        private int yPos;
        private string label;

        private List<Arc> outgoing;

        Ellipse representation;

        public Ellipse Representation
        {
            get { return representation; }
        }

        public Node(int xPos, int yPos, string label)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.label = label;
            this.outgoing = new List<Arc>();
            this.representation = new Ellipse();
        }

        public int XPos
        {
            get { return xPos; }
            set { xPos = value; }
        }

        public int YPos
        {
            get { return yPos; }
            set { yPos = value; }
        }

        public void AddArc(Arc a)
        {
            this.outgoing.Add(a);
        }

        public List<Arc> Outgoing
        {
            get { return outgoing; }
        }

        public double DistanceTo(Node n2)
        {
            int x = xPos - n2.xPos;
            int y = yPos - n2.yPos;
            return Math.Sqrt(x * x + y * y);
        }
    }
}
