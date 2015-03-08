using Effizienze_Graphentheorie.BreadthFirstUtility;
using Effizienze_Graphentheorie.DataExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
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
        

        public void SetSource(Node value, Canvas c = null)
        {
            this.source = value;
            if (c != null)
                value.SetAsSource(c);
            else
                value.IsSource = true;
            value.Representation.Fill = Brushes.LightBlue;
        }

        public void SetDrain(Node value, Canvas c = null)
        {
            this.drain = value;
            if (c != null)
                value.SetAsDrain(c);
            else
                value.IsDrain = true;
            value.Representation.Fill = Brushes.Yellow;
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
            a.Start.AddOutgoingArc(a);
            a.End.AddIncomingArc(a);
        }



        public void Draw()
        {

            foreach (Node n in nodes)
            {

                Ellipse e = n.Representation;
                Canvas.SetTop(e, n.YPos - MainWindow.circleSize / 2);
                Canvas.SetLeft(e, n.XPos - MainWindow.circleSize / 2);

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

        public void PutOnCanvas(Canvas c, int circleSize)
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


            foreach (Node n in nodes)
                c.Children.Add(n.DistanceText);
            
            this.Draw();
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
                n.DistanceText.Foreground = Brushes.Black;
            }
            source.Representation.Fill = Brushes.LightBlue;
            drain.Representation.Fill = Brushes.Yellow;
        }



        private void BuildResidualGraph()
        {
            List<Arc> residual = new List<Arc>();
            foreach (Node n in nodes)
            {
                foreach (Arc a in n.Outgoing)
                {
                    residual.Add(a.GetResidual());
                }
            }
            foreach (Arc a in residual)
            {
                AddArc(a);
            }
        }

        private void DestroyResiduals()
        {
            foreach (Node n in nodes)
            {
                n.Outgoing.RemoveAll(item => item.IsResidual);
                n.Incoming.RemoveAll(item => item.IsResidual);

            }
        }

        public AlgoOutput FordFulkersonStep()
        {
            BuildResidualGraph();
            Arc[] path;
            AlgoOutput output = new AlgoOutput();
            path = DepthFirstSearchPath(source, drain);
            if (path.Length == 0)
            {
                DestroyResiduals();
                output.UsedArcs = new Arc[0];
                output.MaxCap = 0;
                return output;
            }
            int maxCap = int.MaxValue;

            foreach (Arc a in path)
            {
                maxCap = Math.Min(a.MaxCapacity - a.Capacity, maxCap);
            }

            foreach (Arc a in path)
            {
                if (a.IsResidual)
                    a.Origin.Capacity -= maxCap;
                else
                    a.Capacity += maxCap;
            }

            output.UsedArcs = path;
            output.MaxCap = maxCap;

            DestroyResiduals();

            return output;
        }

        public AlgoOutput EdmondsKarpStep()
        {
            this.BuildResidualGraph();
            Arc[] path;
            AlgoOutput output = new AlgoOutput();

            path = BreadthFirstPath(source, drain);
            if(path.Length == 0)
            {
                DestroyResiduals();
                output.UsedArcs = new Arc[0];
                output.MaxCap = 0;
                return output;
            }

            int maxCap = int.MaxValue;

            foreach (Arc a in path)
            {
                maxCap = Math.Min(a.MaxCapacity - a.Capacity, maxCap);
            }

            foreach (Arc a in path)
            {
                if (a.IsResidual)
                    a.Origin.Capacity -= maxCap;
                else
                    a.Capacity += maxCap;
            }

            output.UsedArcs = path;
            output.MaxCap = maxCap;

            DestroyResiduals();
            return output;
        }

        private Arc[] BreadthFirstPath(Node start, Node end)
        {
            Fifo<Node> fifo = new Fifo<Node>(nodes.Count);
            foreach (Node n in nodes)
                n.reset();

            start.IsSeen = true;
            fifo.AddItem(start);

            bool foundPath = false;

            while (!fifo.IsEmpty() && !foundPath)
            {
                Node n = fifo.GetItem();
                foreach (Arc a in n.Outgoing)
                {
                    if (a.End.IsSeen || a.Capacity == a.MaxCapacity)
                        continue;

                    a.End.IsSeen = true;
                    a.End.Preceding = a;
                    fifo.AddItem(a.End);
                    if (a.End == end)
                    {
                        foundPath = true;
                        break;
                    }
                }
            }

            List<Arc> path = new List<Arc>();

            Node prec = end;
            while (prec.Preceding != null)
            {
                path.Add(prec.Preceding);
                prec = prec.Preceding.Start;
            }

            path.Reverse();
            return path.ToArray<Arc>();
        }

        public void InitializePreflowDistance()
        {
            Fifo<Node> fifo = new Fifo<Node>(nodes.Count);
            foreach (Node n in nodes)
                n.reset();

            drain.IsSeen = true;
            fifo.AddItem(drain);
            drain.Distance = 0;

            while (!fifo.IsEmpty())
            {
                Node n = fifo.GetItem();
                foreach (Arc a in n.Incoming)
                {
                    if (a.Start.IsSeen || a.Capacity == a.MaxCapacity)
                        continue;
                    a.Start.Distance = n.Distance + 1;
                    a.Start.IsSeen = true;
                    fifo.AddItem(a.Start);
                }
            }

            source.Distance = nodes.Count;
        }

        public DirectedGraph ConstructDinicSubgraph(Node start, Node end)
        {
            Fifo<Node> fifo = new Fifo<Node>(nodes.Count);
            foreach (Node n in nodes)
                n.reset();

            start.IsSeen = true;
            fifo.AddItem(start);

            bool foundPath = false;

            while (!fifo.IsEmpty())
            {
                Node n = fifo.GetItem();
                foreach (Arc a in n.Outgoing)
                {
                    if (a.Capacity == a.MaxCapacity)
                        continue;
                    if (a.End.IsSeen)
                    {
                        if (a.End.Depth == n.Depth + 1)
                            a.End.AddPreceding(a);
                        continue;
                    }
                    //We don't stop after finding a Path to let the rest of the elements register as preceding
                    if (!foundPath)
                    {
                        a.End.IsSeen = true;
                        a.End.Depth = n.Depth + 1;
                        a.End.AddPreceding(a);
                        fifo.AddItem(a.End);
                        if (a.End == end)
                        {
                            foundPath = true;
                        }
                    }

                }
            }

            if (!foundPath)
                return null;

            Node[] subgraphNodes = new Node[nodes.Count];
            subgraphNodes[end.Label] = new Node(end.XPos, end.YPos, end.Label);
            List<Arc> subplotArcs = new List<Arc>();
            FindArcsAndNodesInPrecedings(end, subplotArcs, subgraphNodes);

            DirectedGraph subgraph = new DirectedGraph();
            for (int i = 0; i < subgraphNodes.Length; i++)
            {
                if (subgraphNodes[i] != null)
                    subgraph.AddNode(subgraphNodes[i]);
            }

            foreach (Arc a in subplotArcs)
            {
                Node startCopy = subgraphNodes[a.Start.Label];
                Node endCopy = subgraphNodes[a.End.Label];
                Arc arcCopy = new Arc(startCopy, endCopy, a.MaxCapacity, false, a);
                arcCopy.Capacity = a.Capacity;
                subgraph.AddArc(arcCopy);
            }

            subgraph.source = subgraphNodes[start.Label];
            subgraph.drain = subgraphNodes[end.Label];

            return subgraph;
        }

        public Arc[] DinicDepthSearch(Node start, Node end)
        {
            Stack<Node> stack = new Stack<Node>(nodes.Count);
            Stack<Arc> ret = new Stack<Arc>(nodes.Count);

            foreach (Node n in nodes)
                n.reset();

            start.IsSeen = true;
            stack.Push(start);

            while (!(stack.IsEmpty() || stack.Peek() == end))
            {
                Node currentNode = stack.Peek();
                Arc currentArc = currentNode.GetNextArc();
                if (currentArc == null)
                {
                    currentNode.RemoveAllIncoming();
                    currentNode.RemoveAllOutgoing();
                    stack.Pop().IsFinished = true;
                    if (!ret.IsEmpty())
                    {
                        ret.Pop();
                    }
                }
                else if (!currentArc.End.IsSeen && currentArc.Capacity != currentArc.MaxCapacity)
                {
                    currentArc.End.IsSeen = true;
                    stack.Push(currentArc.End);
                    ret.Push(currentArc);
                }
            }

            return ret.GetAsArray();
        }

        public AlgoOutput BlockingFlowDinic()
        {
            List<Arc> usedArcs = new List<Arc>();
            Arc[] path = DinicDepthSearch(source, drain);
            int flowIncrease = 0;
            while (path.Length != 0)
            {
                int delta = int.MaxValue;
                foreach (Arc a in path)
                {
                    delta = Math.Min(delta, a.MaxCapacity - a.Capacity);
                }
                foreach (Arc a in path)
                {
                    a.Capacity = a.Capacity + delta;
                    if (a.MaxCapacity == a.Capacity)
                    {
                        a.Start.RemoveArc(a);
                        a.End.RemoveArc(a);
                    }
                }
                flowIncrease += delta;
                usedArcs.AddRange(path);
                path = DinicDepthSearch(source, drain);
            }
            AlgoOutput ret = new AlgoOutput();
            ret.UsedArcs = usedArcs.ToArray();
            ret.MaxCap = flowIncrease;
            return ret;
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
                Arc currentArc = currentNode.GetNextArc();
                if (currentArc == null)
                {
                    if (ret.IsEmpty())
                        return new Arc[0];
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

            return ret.GetAsArray();
        }



        public AlgoOutput DinicStep()
        {
            AlgoOutput ret = new AlgoOutput();
           
            DirectedGraph subGraph = this.ConstructDinicSubgraph(source, drain);
            if (subGraph != null)
            {
                List<Arc> subArcs = new List<Arc>();

                foreach (Node n in subGraph.Nodes)
                {
                    subArcs.AddRange(n.Outgoing);
                }
                ret = subGraph.BlockingFlowDinic();
                foreach (Arc a in subArcs)
                {
                    a.Origin.Capacity = a.Capacity;
                }
                Arc[] usedArcs = new Arc[ret.UsedArcs.Length];
                for (int i = 0; i < usedArcs.Length; i++)
                {
                    usedArcs[i] = ret.UsedArcs[i].Origin;
                }
                ret.UsedArcs = usedArcs;
                return ret;
            }
            ret.UsedArcs = new Arc[0];
            return ret;
        }

        private void FindArcsAndNodesInPrecedings(Node n, List<Arc> subplotArcs, Node[] subplotNodes)
        {
            foreach (Arc a in n.Precedings)
            {

                subplotArcs.Add(a);
                if (subplotNodes[a.Start.Label] == null)
                {
                    subplotNodes[a.Start.Label] = new Node(a.Start.XPos, a.Start.YPos, a.Start.Label);
                    FindArcsAndNodesInPrecedings(a.Start, subplotArcs, subplotNodes);
                }
            }
        }

        public void VisualizeStep(Func<AlgoOutput> GraphAlgorithm, TextBlock text)
        {
            this.resetColor();
            AlgoOutput output = GraphAlgorithm();

            text.Text = "Increased flow by " + output.MaxCap;

            foreach (Arc a in output.UsedArcs)
            {
                if (a.IsResidual)
                {
                    a.Origin.Representation.Stroke = Brushes.Green;
                    if (a.Origin.ReverseArc != null)
                        a.Origin.ReverseArc.Representation.Stroke = Brushes.Green;

                }
                else
                {
                    a.Representation.Stroke = Brushes.Red;
                    if (a.ReverseArc != null)
                        a.ReverseArc.Representation.Stroke = Brushes.Red;
                }
            }
        }

        public void ResetGraph()
        {
            this.PreflowInitialized = false;
            this.resetColor();
            foreach (Node n in nodes)
            {
                foreach (Arc a in n.Outgoing)
                    a.Capacity = 0;
                n.Distance = 0;
                n.DistanceText.Text = "";
            }
                
        }

        public List<Node> Nodes
        {
            get { return this.nodes; }
        }


        public class AlgoOutput
        {
            public Arc[] UsedArcs;
            public int MaxCap;
        }


        List<Node> PreflowNode = new List<Node>();
        bool PreflowInitialized = false;
        public void InitializePreflow()
        {
            InitializePreflowDistance();

            foreach (Arc a in source.Outgoing)
            {
                a.Capacity = a.MaxCapacity;
                if(!a.End.IsDrain && a.MaxCapacity != 0)
                    PreflowNode.Add(a.End);
            }
        }

        public PreflowOutput PreflowStep()
        {
            PreflowOutput ret = new PreflowOutput();
            if (!PreflowInitialized)
            {
                PreflowInitialized = true;
                InitializePreflow();
                ret.distanceIncreased = source;
                return ret;
            }

            if (PreflowNode.Count == 0) return ret;
            Node currentNode = null;
            foreach (Node n in PreflowNode)
            {
                if (n.IsActive())
                {
                    currentNode = n;
                    break;
                }
            }

            if (currentNode == null)
            {
                throw new Exception("Something went wrong");
            }
                


            Arc currentArc = null;
            //suche in den normal ausgehenden 
            foreach (Arc a in currentNode.Outgoing)
            {
                if (a.End.Distance + 1 == a.Start.Distance && a.Capacity != a.MaxCapacity)
                {
                    currentArc = a;
                    break;
                }
            }

            //wir haben in den normalen keinen guten gefunden. Durchsuchen wir den Residualgraph
            if (currentArc == null)
                foreach (Arc a in currentNode.Incoming)
                {
                    if (a.Start.Distance + 1 == a.End.Distance && a.Capacity != 0)
                    {
                        currentArc = a.GetResidual();
                        break;
                    }
                }



            if (currentArc != null)
            {
                if (currentArc.End != source && currentArc.End != drain && currentArc.End.Excess == 0 && !PreflowNode.Contains(currentArc.End))
                    PreflowNode.Add(currentArc.End);
                int difference = Math.Min(currentNode.Excess, currentArc.MaxCapacity - currentArc.Capacity);
                if (difference == 0)
                    throw new Exception("Flow didn't increase");
                if(!currentArc.IsResidual)
                    currentArc.Capacity = currentArc.Capacity + difference;
                else
                    currentArc.Origin.Capacity = currentArc.Origin.Capacity - difference;
                if (currentNode.Excess == 0)
                    PreflowNode.Remove(currentNode);
                ret.saturated = currentArc;
                return ret;
            }
            else
            {
                int dmin = int.MaxValue;
                foreach (Arc a in currentNode.Outgoing)
                {
                    if (a.Capacity < a.MaxCapacity)
                    {
                        dmin = Math.Min(dmin, a.End.Distance);
                    }
                }
                foreach (Arc a in currentNode.Incoming)
                {
                    if (a.Capacity != 0)
                    {
                        dmin = Math.Min(dmin, a.Start.Distance);
                    }
                }
                currentNode.Distance = dmin + 1;
                ret.distanceIncreased = currentNode;
                return ret;
            }
        }

        public class PreflowOutput
        {
            public Node distanceIncreased = null;
            public Arc saturated = null;
        }

        public void VisualizePreFlow(Node start, Node end)
        {
            resetColor();
            PreflowOutput output = PreflowStep();
            Arc[] cut = GetCut();
            foreach (Arc a in cut)
            {
                a.Representation.Stroke = Brushes.Blue;
                if (a.ReverseArc != null)
                    a.ReverseArc.Representation.Stroke = Brushes.Blue;
            }
            if (output.saturated != null)
            {
                output.saturated.Representation.Stroke = Brushes.Red;
                if (output.saturated.ReverseArc != null)
                    output.saturated.ReverseArc.Representation.Stroke = Brushes.Red;
            }
                
            if (output.distanceIncreased != null)
                output.distanceIncreased.DistanceText.Foreground = Brushes.Red;
        }

        public Arc[] GetCut()
        {
            HashSet<Node> reachableNodes = DepthSearchFindNodes(source, drain);
            List<Arc> ret = new List<Arc>();
            foreach (Node n in reachableNodes)
            {
                foreach (Arc a in n.Outgoing)
                {
                    if(!reachableNodes.Contains(a.End))
                        ret.Add(a);
                }
            }
            return ret.ToArray();
        }

        public HashSet<Node> DepthSearchFindNodes(Node start, Node end)
        {
            HashSet<Node> ret = new HashSet<Node>();
            Stack<Node> stack = new Stack<Node>(nodes.Count);

            foreach (Node n in nodes)
                n.reset();

            start.IsSeen = true;
            stack.Push(start);
            ret.Add(start);

            while (!stack.IsEmpty())
            {
                Node currentNode = stack.Peek();
                Arc currentArc = currentNode.GetNextArc();
                if (currentArc == null)
                {
                    stack.Pop().IsFinished = true;
                    continue;
                }

                if (currentArc.End.IsSeen == false && currentArc.Capacity != currentArc.MaxCapacity)
                {
                    currentArc.End.IsSeen = true;
                    stack.Push(currentArc.End);
                    ret.Add(currentArc.End);
                }

            }

            return ret;
        }

        public JsonGraph GetJsonGraph()
        {
            JsonGraph json = new JsonGraph();
            JsonNode[] jsonNodes = new JsonNode[nodes.Count];
            JsonArc[] jsonArcs;

            int numberOfArcs = 0;
            foreach (Node n in nodes)
            {
                foreach (Arc a in n.Outgoing)
                    numberOfArcs++;
            }

            jsonArcs = new JsonArc[numberOfArcs];

            int nodeCount = 0;
            int arcCount = 0;
            foreach (Node n in nodes)
            {
                JsonNode singleNode = new JsonNode();
                singleNode.label = n.Label;
                singleNode.xPos = n.XPos;
                singleNode.yPos = n.YPos;
                jsonNodes[nodeCount++] = singleNode;
                foreach (Arc a in n.Outgoing)
                {
                    JsonArc singleArc = new JsonArc();
                    singleArc.startLabel = a.Start.Label;
                    singleArc.endLabel = a.End.Label;
                    singleArc.maxCapacity = a.MaxCapacity;
                    jsonArcs[arcCount++] = singleArc;
                }
            }

            json.nodes = jsonNodes;
            json.arcs = jsonArcs;
            //json.source = this.source.Label;
            //json.drain = this.drain.Label;

            return json;
        }

        public static DirectedGraph ConstructGraphFromJson(string Json)
        {
            JsonGraph jsonGraph = new JavaScriptSerializer().Deserialize<JsonGraph>(Json);
            DirectedGraph copiedGraph = new DirectedGraph();

            Node[] copiedNodes = new Node[jsonGraph.nodes.Length];
            Arc[] copiedArcs = new Arc[jsonGraph.arcs.Length];

            for (int i = 0; i < copiedNodes.Length; i++)
            {
                copiedNodes[i] = new Node(jsonGraph.nodes[i].xPos, jsonGraph.nodes[i].yPos, jsonGraph.nodes[i].label);
            }

            for (int i = 0; i < copiedArcs.Length; i++)
            {
                copiedArcs[i] = new Arc(copiedNodes[jsonGraph.arcs[i].startLabel], copiedNodes[jsonGraph.arcs[i].endLabel], jsonGraph.arcs[i].maxCapacity);
            }

            foreach (Node n in copiedNodes)
            {
                copiedGraph.AddNode(n);
            }
            foreach (Arc a in copiedArcs)
            {
                copiedGraph.AddArc(a);
            }

            copiedGraph.SetSource(copiedNodes[jsonGraph.source]);
            copiedGraph.SetDrain(copiedNodes[jsonGraph.drain]);

            return copiedGraph;
        }

        public Node Source
        {
            get { return source; }
        }
        public Node Drain
        {
            get { return drain; }
        }
    }
}
