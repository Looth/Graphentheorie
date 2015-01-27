using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Effizienze_Graphentheorie.Graph
{
    class DirectedGraph
    {
        private List<Node> nodes;
        private Node source;
        private Node drain;

        public Node Source
        {
            get { return source; }
            set
            {
                this.source = value;
                value.Representation.Fill = Brushes.Blue;
            }
        }

        public Node Drain
        {
            get { return Drain; }
            set
            {
                this.drain = value;
                value.Representation.Fill = Brushes.Yellow;
            }
        }



        public DirectedGraph()
        {
            nodes = new List<Node>();
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public void AddArc(Arc a)
        {
            a.Start.AddArc(a);
        }



        public void Draw(int circleSize)
        {

            foreach (Node n in nodes)
            {

                Ellipse e = n.Representation;
                Canvas.SetTop(e, n.YPos - circleSize / 2);
                Canvas.SetLeft(e, n.XPos - circleSize / 2);

                foreach (Arc a in n.Outgoing)
                {
                    Line l = a.Representation;


                    l.X1 = a.Start.XPos;
                    l.Y1 = a.Start.YPos;

                    l.X2 = a.End.XPos;
                    l.Y2 = a.End.YPos;

                    //c.Children.Add(getCurve(new Point(a.Start.XPos, a.Start.YPos), new Point(a.End.XPos, a.End.YPos)));


                    TextBlock capacity = a.CapacityText;
                    capacity.Background = Brushes.White;

                    Canvas.SetTop(capacity, l.Y1 + 0.8 * (l.Y2 - l.Y1) - capacity.ActualHeight / 2);
                    Canvas.SetLeft(capacity, l.X1 + 0.8 * (l.X2 - l.X1) - capacity.ActualWidth / 2);
                }
            }

        }

        public void putOnCanvas(Canvas c, int circleSize)
        {
            c.Children.Clear();

            //add arcs first so they are the lowest element
            foreach (Node n in nodes)
                foreach (Arc a in n.Outgoing)
                    c.Children.Add(a.Representation);

            //now add capacities so they lay on top of arcs
            foreach (Node n in nodes)
                foreach (Arc a in n.Outgoing)
                    c.Children.Add(a.CapacityText);

            //at last add nodes themself so they will be on top of arcs and capacities 
            foreach (Node n in nodes)
                c.Children.Add(n.Representation);
            this.Draw(circleSize);
        }


        //not used at the moment, might be used to generate curved arcs
        private Shape getCurve(Point from, Point to)
        {
            ArcSegment arcs = new ArcSegment(to, new Size(800, 800), 45, false, SweepDirection.Clockwise, true);

            PathSegmentCollection pscollection = new PathSegmentCollection();
            pscollection.Add(arcs);

            PathFigure pf = new PathFigure();
            pf.Segments = pscollection;
            pf.StartPoint = from;

            PathFigureCollection pfcollection = new PathFigureCollection();
            pfcollection.Add(pf);

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = pfcollection;

            Path path = new Path();
            path.Data = pathGeometry;
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 1;
            return path;
        }


        public Arc[] DepthFirstSearchPath(Node start, Node end)
        {
            //First lets start preprocessing/induction basis

            Stack<Node> stack = new Stack<Node>(nodes.Count);
            Stack<Arc> ret = new Stack<Arc>(nodes.Count);

            foreach (Node n in nodes)
                n.reset();

            start.IsSeen = true;
            stack.Push(start);

            while (!(stack.Peek() == end || stack.IsEmpty()))
            {
                Node currentNode = stack.Peek();
                Arc currentArc = currentNode.getNextArc();
                if (currentArc == null)
                {
                    ret.Pop();
                    stack.Pop().IsFinished = true;
                    continue;
                }

                if (currentArc.End.IsSeen == false && currentArc.Capacity != currentArc.MaxCapacity)
                {
                    currentArc.End.IsSeen = true;
                    ret.Push(currentArc);
                    stack.Push(currentArc.End);

                }

            }

            if (stack.Peek().IsFinished)
                throw new Exception("Could not find a Path");

            return ret.GetAsArray();
        }

        private void resetColor()
        {
            foreach (Node n in nodes)
            {
                foreach (Arc a in n.Outgoing)
                {
                    a.Representation.Stroke = Brushes.Black;
                }
                n.Representation.Fill = Brushes.White;
                n.Representation.Stroke = Brushes.Black;
            }
            source.Representation.Fill = Brushes.Blue;
            drain.Representation.Fill = Brushes.Yellow;
        }

        public void FordFulkersonStep()
        {
            this.resetColor();
            Arc[] path = DepthFirstSearchPath(source, drain);
            int maxCap = int.MaxValue;
            foreach (Arc a in path)
            {
                maxCap = Math.Min(a.MaxCapacity, maxCap);
                a.Representation.Stroke = Brushes.Red;
                a.ReverseArc.Representation.Stroke = Brushes.Red;
            }

            foreach (Arc a in path)
            {
                a.Capacity = maxCap;
            }

        }
    }
}
