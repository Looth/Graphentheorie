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
        private List<Arc> incoming;
        private bool isSource;
        private bool isDrain;

        /**
         * These Values are used for Representation
         */
        private int xPos;
        private int yPos;
        private int label;
        private Ellipse representation;
        private TextBlock distanceText;

        /*
         * These Values are used for Depth/Breadth first search
         */
        private bool isSeen;
        private bool isFinished;
        private int count;
        private Arc preceding;

        /*
         *  These Values are used for SubplotConstruction/Dinic
         */
        private List<Arc> precedings;
        private int depth;

        /*
         * These Values are used for Preflow-Push
         */
        private int distance = int.MaxValue;

        public Ellipse Representation
        {
            get { return representation; }
        }

        public Node(int xPos, int yPos, int label)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.label = label;
            this.outgoing = new List<Arc>();
            this.incoming = new List<Arc>();
            this.representation = new Ellipse();
            this.distanceText = new TextBlock();
            this.distanceText.IsHitTestVisible = false;
            Canvas.SetLeft(distanceText, xPos);
            Canvas.SetTop(distanceText, yPos);
            isSource = false;
            isDrain = false;

            representation.Height = MainWindow.circleSize;
            representation.Width = MainWindow.circleSize;
            representation.Stroke = Brushes.Black;
            representation.StrokeThickness = 2;
            representation.Fill = Brushes.White;
            representation.DataContext = this;
        }

        public int XPos
        {
            get { return xPos; }
            set 
            {
                xPos = value;
                Canvas.SetLeft(distanceText, xPos);
            }
        }

        public int YPos
        {
            get { return yPos; }
            set 
            {
                yPos = value;
                Canvas.SetTop(distanceText, yPos);

            }
        }

        public void AddOutgoingArc(Arc a)
        {
            if (a.Start != this)
                throw new Exception("Arc was added wrong to Node");
            this.outgoing.Add(a);
        }

        public void AddIncomingArc(Arc a)
        {
            if (a.End != this)
                throw new Exception("Arc was added wrong to Node");
            this.incoming.Add(a);
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
            precedings = new List<Arc>();
            depth = -1;
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

        public List<Arc> RemoveAllOutgoing()
        {
            List<Arc> removed = outgoing;
            outgoing = new List<Arc>();
            foreach (Arc a in removed)
            {
                a.End.RemoveArc(a);
            }
            return removed;
        }

        public List<Arc> RemoveAllIncoming()
        {
            List<Arc> removed = incoming;
            incoming = new List<Arc>();
            foreach (Arc a in removed)
            {
                a.Start.RemoveArc(a);
            }
            return removed;
        }

        public void RemoveArc(Arc a)
        {
            if (a.End == this)
            {
                incoming.Remove(a);
            }
            else if (a.Start == this)
            {
                if (outgoing.IndexOf(a) < count)
                    count--;
                outgoing.Remove(a);
            }
            else
            {
                throw new Exception("This Arc does not belong to the Node");
            }
        }

        public int Label
        {
            get { return label; }
        }
        public List<Arc> Precedings
        {
            get { return precedings; }
        }


        public Arc GetNextArc()
        {
            if (count >= outgoing.Count)
                return null;
            return outgoing[count++];
        }

        public int Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        public void AddPreceding(Arc a)
        {
            precedings.Add(a);
        }

        public TextBlock DistanceText
        {
            get { return distanceText; }
        }

        public bool IsActive()
        {
            int excess = 0;
            foreach (Arc a in Incoming)
            {
                excess += a.Capacity;
            }
            foreach (Arc a in Outgoing)
            {
                excess -= a.Capacity;
            }
            return excess > 0;
        }

        public int Distance
        {
            set 
            { 
                distance = value;
                distanceText.Text = value.ToString();
            }
            get { return distance; }
        }

        public List<Arc> Incoming
        {
            get { return incoming; }
            set { incoming = value; }
        }

        public int Excess
        {
            get
            {
                if (IsSource || IsDrain)
                    return 0;
                int excess = 0;
                foreach (Arc a in Incoming)
                {
                    excess += a.Capacity;
                }
                foreach (Arc a in Outgoing)
                {
                    excess -= a.Capacity;
                }
                return Math.Max(excess, 0);
            }
        }

        public bool IsDrain
        {
            get { return isDrain; }
            set
            {
                isDrain = value; 
                List<Arc> removed = this.RemoveAllOutgoing();
                foreach (Arc a in removed)
                {
                    a.Capacity = 0;
                    a.MaxCapacity = 0;
                }
            }
        }

        public bool IsSource
        {
            get { return isSource; }
            set 
            { 
                isSource = value; 
                List<Arc> removed = this.RemoveAllIncoming();
                foreach (Arc a in removed)
                {
                    a.Capacity = 0;
                    a.MaxCapacity = 0;
                }
            }
        }

        public void SetAsSource(Canvas c)
        {
            this.isSource = true;
            List<Arc> removed = this.RemoveAllIncoming();
            foreach (Arc a in removed)
            {
                a.Capacity = 0;
                a.MaxCapacity = 0;
                c.Children.Remove(a.Representation);
                c.Children.Remove(a.CapacityText);
            }
        }
        public void SetAsDrain(Canvas c)
        {
            this.isDrain = true;
            List<Arc> removed = this.RemoveAllOutgoing();
            foreach (Arc a in removed)
            {
                a.Capacity = 0;
                a.MaxCapacity = 0;
                c.Children.Remove(a.Representation);
                c.Children.Remove(a.CapacityText);
            }
        }
    }
}
