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
        int circleSize = 20;
        public MainWindow()
        {
            InitializeComponent();
        }


        private void GenerateGraph(object sender, RoutedEventArgs evt)
        {
            

            RndGraphGenerator generator = new RndGraphGenerator(circleSize, (int) canvas.ActualHeight, (int) canvas.ActualWidth);
            graph = generator.GenerateGraph(20, 100);
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

    }
}
