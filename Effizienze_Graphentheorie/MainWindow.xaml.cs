using Effizienze_Graphentheorie.BreadthFirstUtility;
using Effizienze_Graphentheorie.DataExport;
using Effizienze_Graphentheorie.Graph;
using Effizienze_Graphentheorie.Testumgebung;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
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
        public const int circleSize = 50;
        int nodeCount = 10;
        public MainWindow()
        {
            InitializeComponent();
        }


        private void GenerateGraph(object sender, RoutedEventArgs evt)
        {
            RndGraphGenerator generator = new RndGraphGenerator(circleSize, (int) canvas.ActualHeight, (int) canvas.ActualWidth);
            graph = generator.GenerateGraph(nodeCount, 50);
            graph.PutOnCanvas(canvas, circleSize);
            source = null;
            drain = null;

            ChooseSource();

            /*
            var json = new JavaScriptSerializer().Serialize(graph.GetJsonGraph());
            Console.WriteLine(json);

            DirectedGraph copy = DirectedGraph.ConstructGraphFromJson(json);
            */
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
            graph.SetSource(source, canvas);

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
            graph.SetDrain(drain,canvas);

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
            graph.Draw();
        }

        private void FordFulkersonStep(object sender, RoutedEventArgs e)
        {
            if (source != null && drain != null)
                graph.VisualizeStep(graph.FordFulkersonStep, text);
        }

        private void EdmondsKarpStep(object sender, RoutedEventArgs e)
        {
            if (source != null && drain != null)
                graph.VisualizeStep(graph.EdmondsKarpStep, text);
        }


        DirectedGraph BuildSpecialGraph()
        {
            DirectedGraph g = new DirectedGraph();
            Node n1 = new Node(10, 150, 0);
            Node n2 = new Node(200, 30, 1);
            Node n3 = new Node(200, 120, 2);
            Node n4 = new Node(400, 150, 3);
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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            nodeCount = (int)(sender as Slider).Value;
        }

        bool displaySub = false;
        private void Subgraph(object sender, RoutedEventArgs e)
        {
            DirectedGraph subgraph;
            subgraph = graph.ConstructDinicSubgraph(source, drain);
            if (subgraph == null)
                return;
            if (source != null && drain != null)
            {
                canvas.Children.Clear();
            }
            if (!displaySub)
            {
                subgraph.PutOnCanvas(canvas, circleSize);

                displaySub = true;
            }
            else
            {
                graph.PutOnCanvas(canvas, circleSize);
                displaySub = false;
            }
            
        }

        private void DinicStep(object sender, RoutedEventArgs e)
        {
            if (source != null && drain != null)
                graph.VisualizeStep(graph.DinicStep, text);
        }

        private void ResetGraph(object sender, RoutedEventArgs e)
        {
            if(graph != null)
                graph.ResetGraph();
        }

        private void PreflowStep(object sender, RoutedEventArgs e)
        {
            if (source != null && drain != null)
                graph.VisualizePreFlow(source, drain);
        }

        private void TryTesting(object sender, RoutedEventArgs e)
        
        {

            TestWindow w = new TestWindow();
            w.Show();
        }

        private void ReadJSON(object sender, RoutedEventArgs e)
        {
            JsonField.Visibility = System.Windows.Visibility.Visible;
        }

        private void Send(object sender, RoutedEventArgs e)
        {
            JsonField.Visibility = System.Windows.Visibility.Collapsed;
            string json = JsonTextBox.Text;
            JsonPerformance perf = null;
            try
            {
                perf = new JavaScriptSerializer().Deserialize<JsonPerformance>(json);
            }
            catch (Exception)
            {
                return;
            }
            if (perf == null) return;
            graph = DirectedGraph.ConstructGraphFromJson(perf.graph);
            graph.PutOnCanvas(canvas, circleSize);

            foreach (UIElement elem in canvas.Children)
            {
                if (elem is Ellipse)
                {
                    elem.MouseDown += StartDrag;
                }
            }

            this.source = graph.Source;
            this.drain = graph.Drain;
        }
    }
}
