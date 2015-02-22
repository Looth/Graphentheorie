using Effizienze_Graphentheorie.Graph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Effizienze_Graphentheorie
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Node source = null;
        Node drain = null;
        DirectedGraph graph = null;
        int circleSize = 50;
        public MainWindow()
        {
            InitializeComponent();
        }


        private void GenerateGraph(object sender, RoutedEventArgs evt)
        {
            

            RndGraphGenerator generator = new RndGraphGenerator(circleSize, (int) canvas.ActualHeight, (int) canvas.ActualWidth);
            graph = generator.GenerateGraph(10, 50, circleSize);
            graph.putOnCanvas(canvas, circleSize);
            source = null;
            drain = null;

            ChooseSource();
        }

        private void ChooseSource()
        {
            text.Text = "Please select a source";
            foreach (UIElement e in canvas.Children)
            {
                if (e is Ellipse)
                {
                    e.MouseDown += SetAsSource;
                }
            }
        }

        private void SetAsSource(object sender, MouseButtonEventArgs evt)
        {
            text.Text = "Please select a drain";
            Ellipse ellipse = sender as Ellipse;
            source = ellipse.DataContext as Node;
            graph.Source = source;

            foreach (UIElement e in canvas.Children)
            {
                if (e is Ellipse)
                {
                    e.MouseDown -= SetAsSource;
                    e.MouseDown += SetAsDrain;
                }
            }
        }

        private void SetAsDrain(object sender, MouseButtonEventArgs evt)
        {
            Ellipse ellipse = sender as Ellipse;
            if (ellipse.DataContext == source) return;
            text.Text = "Please select a algorithm";

            drain = ellipse.DataContext as Node;
            graph.Drain = drain;

            foreach (UIElement e in canvas.Children)
            {
                if (e is Ellipse)
                {
                    e.MouseDown -= SetAsDrain;
                    e.MouseDown += StartDrag;
                }
            }
        }

        void StartDrag(object sender, MouseButtonEventArgs e)
        {
            Ellipse el = sender as Ellipse;
            el.MouseMove += DragMove;
            el.MouseUp += StopDrag;
        }

        void StopDrag(object sender, MouseButtonEventArgs e)
        {
            Ellipse el = sender as Ellipse;
            el.MouseMove -= DragMove;
        }

        void DragMove(object sender, MouseEventArgs e)
        {
            Ellipse el = sender as Ellipse;
            Node n = el.DataContext as Node;
            n.XPos = (int)e.GetPosition(canvas).X;
            n.YPos = (int)e.GetPosition(canvas).Y;
            graph.Draw(circleSize);
        }

        private void FordFulkersonStep(object sender, RoutedEventArgs e)
        {
            if (source != null && drain != null)
                graph.FordFulkersonStep(text);
        }

        private void EdmondsKarpStep(object sender, RoutedEventArgs e)
        {
            if (source != null && drain != null)
                graph.EdmondsKarpStep(text);
        }


        DirectedGraph BuildSpecialGraph()
        {
            DirectedGraph g = new DirectedGraph();
            Node n1 = new Node(10, 150, "1", 20);
            Node n2 = new Node(200, 30, "2", 20);
            Node n3 = new Node(200, 120, "3", 20);
            Node n4 = new Node(400, 150, "4", 20);
            Arc a1 = new Arc(n1, n2, 1);
            Arc a2 = new Arc(n1, n3, 1);
            Arc a3 = new Arc(n2, n3, 1);
            Arc a4 = new Arc(n2, n4, 1);
            Arc a5 = new Arc(n3, n4, 1);

            g.AddNode(n1);
            g.AddNode(n2);
            g.AddNode(n3);
            g.AddNode(n4);

            g.AddArc(a1);
            g.AddArc(a3);
            g.AddArc(a5);
            g.AddArc(a2);
            g.AddArc(a4);
            return g;
        }
    }
}
