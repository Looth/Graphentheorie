using Effizienze_Graphentheorie.Testumgebung;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Effizienze_Graphentheorie
{
    /// <summary>
    /// Interaktionslogik für TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private void AddNewRow_Click(object sender, RoutedEventArgs e)
        {
            InstancePanel.Children.Add(new TextBox());
            NodePanel.Children.Add(new TextBox());
            CapacityPanel.Children.Add(new TextBox());
        }

        private void Evaluate_Click(object sender, RoutedEventArgs e)
        {
            List<Tripel> config = new List<Tripel>();
            
            for (int i = 0; i < InstancePanel.Children.Count; i++)
            {
                TextBox InstanceBox = InstancePanel.Children[i] as TextBox;
                TextBox NodeBox = NodePanel.Children[i] as TextBox;
                TextBox CapacityBox = CapacityPanel.Children[i] as TextBox;

                int instances;
                int nodes;
                int maxCapacity;
                if(int.TryParse(InstanceBox.Text, out instances) && int.TryParse(NodeBox.Text, out nodes) && int.TryParse(CapacityBox.Text, out maxCapacity))
                {
                    Tripel t = new Tripel();
                    t.instances = instances;
                    t.nodeCount = nodes;
                    t.maxCapacity = maxCapacity;
                    config.Add(t);
                }
            }

            TestAlgorithms test = new TestAlgorithms(config);

            Result.Children.Clear();

            foreach (Tripel t in config)
            {
                DockPanel res = new DockPanel();
                Border b = new Border();
                b.IsHitTestVisible = false;
                b.BorderThickness = new Thickness(1);
                b.Background = Brushes.White;
                b.BorderBrush = Brushes.Black;
                b.Height = 24;
                TextBlock allFeasible = new TextBlock();
                b.Child = allFeasible;
                allFeasible.Width = 110;
                allFeasible.Text = test.CheckIfAllFeasible().ToString();
                res.Children.Add(b);

                b = new Border();
                b.IsHitTestVisible = false;
                b.Background = Brushes.White;
                b.BorderThickness = new Thickness(1);
                b.BorderBrush = Brushes.Black;
                b.Height = 24;
                TextBlock identicalFlow = new TextBlock();
                identicalFlow.Text = test.CheckIfFlowIsIdentical().ToString();
                b.Child = identicalFlow;
                res.Children.Add(b);


                b = new Border();
                b.Background = Brushes.White;
                b.IsHitTestVisible = false;
                b.Visibility = System.Windows.Visibility.Collapsed;
                DockPanel.SetDock(b, Dock.Bottom);
                b.BorderThickness = new Thickness(1);
                b.BorderBrush = Brushes.Red;
                StackPanel gEval = new StackPanel();
                b.Child = gEval;
               
                Result.Children.Add(res);
            }
        }
    }
}
