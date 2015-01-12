﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Effizienze_Graphentheorie.Graph
{
    class Arc
    {
        Node start;
        Node end;
        int maxCapacity;
        int capacity;

        Line representation;
        TextBlock label;

        public Line Representation
        {
            get { return representation; }
        }

        public TextBlock Label
        {
            get { return label; }
        }

        public Arc(Node start, Node end, int maxCapacity)
        {
            this.start = start;
            this.end = end;
            this.maxCapacity = maxCapacity;
            this.capacity = 0;
            this.representation = new Line();
            this.label = new TextBlock();
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
            get { return capacity; }
        }
    }
}