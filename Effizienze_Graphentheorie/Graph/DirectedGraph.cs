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
                e.Height = circleSize;
                e.Width = circleSize;
                e.Stroke = Brushes.Black;
                e.StrokeThickness = 2;
                e.Fill = Brushes.White;
                e.DataContext = n;

                foreach (Arc a in n.Outgoing)
                {
                    Line l = a.Representation;

                    l.Stroke = Brushes.Black;
                    l.StrokeThickness = 1;

                    l.X1 = a.Start.XPos;
                    l.Y1 = a.Start.YPos;

                    l.X2 = a.End.XPos;
                    l.Y2 = a.End.YPos;

                    //c.Children.Add(getCurve(new Point(a.Start.XPos, a.Start.YPos), new Point(a.End.XPos, a.End.YPos)));


                    TextBlock capacity = a.CapacityText;
                    capacity.Text = a.Capacity + "/" + a.MaxCapacity;
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
            foreach(Node n in nodes)
                foreach (Arc a in n.Outgoing)
                    c.Children.Add(a.Representation);

            //now add capacities so they lay on top of arcs
            foreach(Node n in nodes)
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
            ArcSegment arcs = new ArcSegment(to, new Size(800,800), 45, false, SweepDirection.Clockwise, true);
            
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

            Stack<DepthFirstNode> stack = new Stack<DepthFirstNode>(nodes.Count);

            //give each node 2 flags and add a outgoing arc and a 
            DepthFirstNode[] depthNodes = new DepthFirstNode[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                depthNodes[i] = new DepthFirstNode(nodes[i]);
                if (nodes[i] == start)
                {
                    depthNodes[i].IsSeen = true;
                    stack.Push(depthNodes[i]);
                }
            }

            throw new NotImplementedException("don't be lazy like me!");
        }
    }
}
