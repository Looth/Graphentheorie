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
        private List<Arc> arcs;

        public DirectedGraph()
        {
            nodes = new List<Node>();
            arcs = new List<Arc>();
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public void AddArc(Arc a)
        {
            arcs.Add(a);
        }



        public void Draw(int circleSize)
        {

            foreach (Arc a in arcs)
            {
                Line l = a.Representation;

                l.Stroke = Brushes.Black;
                l.StrokeThickness = 1;

                l.X1 = a.Start.XPos;
                l.Y1 = a.Start.YPos;

                l.X2 = a.End.XPos;
                l.Y2 = a.End.YPos;

                //c.Children.Add(getCurve(new Point(a.Start.XPos, a.Start.YPos), new Point(a.End.XPos, a.End.YPos)));


                TextBlock capacity = a.Label;
                capacity.Text = a.Capacity + "/" + a.MaxCapacity;
                capacity.Background = Brushes.White;

                Canvas.SetTop(capacity, l.Y1 + 0.8 * (l.Y2 - l.Y1) - capacity.ActualHeight / 2);
                Canvas.SetLeft(capacity, l.X1 + 0.8 * (l.X2 - l.X1) - capacity.ActualWidth / 2);


            }

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
            }
            
        }

        public void putOnCanvas(Canvas c, int circleSize)
        {
            c.Children.Clear();
            foreach (Arc a in arcs)
                c.Children.Add(a.Representation);
            foreach (Arc a in arcs)
                c.Children.Add(a.Label);
            foreach (Node n in nodes)
                c.Children.Add(n.Representation);
            this.Draw(circleSize);
        }

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

    }
}
