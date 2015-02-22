using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Effizienze_Graphentheorie.Graph
{
    class Node
    {
        private List<Arc> outgoing;

        /**
         * These Values are used for Representation
         */ 
        private int xPos;
        private int yPos;
        private string label;
        Ellipse representation;
        
        /*
         * These Values are used for Depth/Breadth first search
         */
        private bool isSeen;
        private bool isFinished;
        int count;
        Arc preceding;


        public Ellipse Representation
        {
            get { return representation; }
        }

        public Node(int xPos, int yPos, string label, int circleSize)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.label = label;
            this.outgoing = new List<Arc>();
            this.representation = new Ellipse();

            representation.Height = circleSize;
            representation.Width = circleSize;
            representation.Stroke = Brushes.Black;
            representation.StrokeThickness = 2;
            representation.Fill = Brushes.White;
            representation.DataContext = this;
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

        public bool IsSeen
        {
            get { return isSeen; }
            set { isSeen = value; }
        }

        public bool IsFinished
        {
            get { return isFinished; }
            set { isFinished = value; }
        }

        public void reset()
        {
            count = 0;
            IsFinished = false;
            isSeen = false;
            preceding = null;
        }

        public Arc Preceding
        {
            set
            {
                if (value.End != this)
                    throw new Exception("This can't be the preceding Arc");
                preceding = value;
            }
            get { return preceding; }
        }

        public Arc getNextArc()
        {
            if (count >= outgoing.Count())
                return null;
            return outgoing[count++];
        }


    }
}
