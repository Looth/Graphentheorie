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
    class Arc
    {
        private Node start;
        private Node end;
        private int maxCapacity;
        private int capacity;
        private Arc reverseArc;

        /*
         * used for Residual Arcs
         */
        private bool isResidual;
        private Arc origin;

        

        private Line representation;
        private TextBlock capacityText;

        

        public Arc(Node start, Node end, int maxCapacity, bool isResidual = false, Arc origin = null)
        {
            this.isResidual = isResidual;
            this.origin = origin;
            this.start = start;
            this.end = end;
            this.maxCapacity = maxCapacity;
            this.capacity = 0;
            this.representation = new Line();
            this.capacityText = new TextBlock();

            representation.Stroke = Brushes.Black;
            representation.StrokeThickness = 1;
            capacityText.Text = "0/" + maxCapacity;

        }

        public Node Start
        {
            get { return start; }
        }

        public Node End
        {
            get { return end; }
        }

        public int MaxCapacity
        {
            get { return maxCapacity; }
        }
        public int Capacity
        {
            set
            {
                this.capacity = value;
                capacityText.Text = Capacity + "/" + MaxCapacity;
            }
            get { return capacity; }
        }
        public Line Representation
        {
            get { return representation; }
        }

        public TextBlock CapacityText
        {
            get { return capacityText; }
        }

        public Arc ReverseArc
        {
            get { return reverseArc; }
            set { reverseArc = value; }
        }

        public bool IsResidual
        {
            get { return isResidual; }
        }

        public Arc Origin
        {
            get { return origin; }
        }

        public Arc GetResidual()
        {
            if (this.isResidual)
                throw new Exception("GetResidual was called with a ResidualArc");
            Arc residual = new Arc(this.end, this.start, this.maxCapacity, true, this);
            residual.Capacity = this.maxCapacity - this.capacity;
            return residual;
        }
    }
}
