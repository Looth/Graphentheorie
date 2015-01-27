using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effizienze_Graphentheorie.Graph
{
    class DepthFirstNode
    {
        private bool isSeen;
        private bool isFinished;
        private Node node;
        private int outgoingCount;

        public DepthFirstNode(Node n)
        {
            isSeen = false;
            isFinished = false;
            node = n;
            outgoingCount = 0;
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

        internal Node Node
        {
            get { return node; }
        }

        public Arc currentArc()
        {
            if (outgoingCount == this.node.Outgoing.Count)
            {
                return null;
            }
            return node.Outgoing[outgoingCount];

        }

        public void nextArc()
        {
            outgoingCount++;
        }
    }
}
