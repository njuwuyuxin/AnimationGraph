using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class AnimationGraphView : GraphView
    {
        private const string k_StyleSheetPrefix = "Assets/Scripts/AnimationGraph/Editor/StyleSheet/";
        private AnimationGraphAsset m_AnimationGraphAsset;
        public NodeInspector inspector => m_Inspector;
        private NodeInspector m_Inspector;

        public AnimationGraphView(NodeInspector inspector)
        {
            m_Inspector = inspector;
            AddGridBackground();
            AddManipulators();
            AddStyleSheet();
            RegisterCallbacks();
        }

        private void AddGridBackground()
        {
            GridBackground bg = new GridBackground();
            Insert(0, bg);
            bg.StretchToParentSize();
        }

        private void AddManipulators()
        {
            this.AddManipulator(new ContentDragger());
            SetupZoom(ContentZoomer.DefaultMinScale,ContentZoomer.DefaultMaxScale);
            this.AddManipulator(CreateContextualMenu());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
        }
        
        private IManipulator CreateContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent =>
                {
                    menuEvent.menu.AppendAction(
                        "Add FinalPose Node",
                        actionEvent => CreateDefaultNode(ENodeType.FinalPoseNode, MouseToViewPosition(actionEvent.eventInfo.mousePosition))
                    );
                    menuEvent.menu.AppendAction(
                        "Add AnimationClip Node",
                        actionEvent => CreateDefaultNode(ENodeType.AnimationClipNode, MouseToViewPosition(actionEvent.eventInfo.mousePosition))
                    );
                });
            return contextualMenuManipulator;
        }

        private void AddStyleSheet()
        {
            var graphViewStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StyleSheetPrefix + "AnimationGraphView.uss");
            if (graphViewStyleSheet != null)
            {
                styleSheets.Add(graphViewStyleSheet);
            }
        }

        private void RegisterCallbacks()
        {
            RegisterCallback<DragEnterEvent>(OnDragEnter);
            RegisterCallback<DragLeaveEvent>(OnDragLeave);
            RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            RegisterCallback<DragPerformEvent>(OnDragPerform);
            RegisterCallback<DragExitedEvent>(OnDragExit);
        }

        void OnDragEnter(DragEnterEvent evt)
        {
        }
        
        void OnDragLeave(DragLeaveEvent evt)
        {
            
        }
        
        void OnDragUpdate(DragUpdatedEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }
        
        private void OnDragPerform(DragPerformEvent evt)
        {
            DragAndDrop.AcceptDrag();
            
            var parameterCard = DragAndDrop.GetGenericData("parameterCard") as ParameterCard;
            if (parameterCard != null)
            {
                CreateParameterNode(parameterCard, MouseToViewPosition(evt.mousePosition));
            }
            
            Debug.Log(parameterCard.parameterName);
        }
        
        void OnDragExit(DragExitedEvent evt)
        { 
        }

        private Vector2 MouseToViewPosition(Vector2 mousePosition)
        {
            Vector2 graphViewPosition = VisualElementExtensions.LocalToWorld(this, new Vector2(transform.position.x,transform.position.y));
            return mousePosition - graphViewPosition;
        }
        
        private GraphNode CreateNode(ENodeType nodeType, Vector2 position)
        {
            GraphNode node = null;
            switch (nodeType)
            {
                case ENodeType.FinalPoseNode: node = new FinalPoseNode(this, position);
                    break;
                case ENodeType.AnimationClipNode: node = new AnimationClipNode(this, position);
                    break;
                case ENodeType.BoolValueNode: node = new BoolValueGraphNode(this, position);
                    break;
                case ENodeType.IntValueNode: node = new IntValueGraphNode(this, position);
                    break;
                case ENodeType.FloatValueNode: node = new FloatValueGraphNode(this, position);
                    break;
                case ENodeType.StringValueNode: node = new StringValueGraphNode(this, position);
                    break;
                default: node = new GraphNode(this, position);
                    break;
            }

            if (node != null)
            {
                AddElement(node);
            }
            else
            {
                Debug.LogError("[AnimationGraph][GraphView]: Create Node failed, nodeType:" + nodeType);
            }

            return node;
        }

        private GraphNode CreateDefaultNode(ENodeType nodeType, Vector2 position)
        {
            var node = CreateNode(nodeType, position);
            node.InitializeDefault();
            return node;
        }

        private GraphNode CreateParameterNode(ParameterCard parameterCard, Vector2 position)
        {
            GraphNode node = null;
            if (parameterCard is BoolParameterCard)
            {
                node = CreateNode(ENodeType.BoolValueNode, position);
            }
            else if (parameterCard is IntParameterCard)
            {
                node = CreateNode(ENodeType.IntValueNode, position);
            }
            else if (parameterCard is FloatParameterCard)
            {
                node = CreateNode(ENodeType.FloatValueNode, position);
            }
            else if (parameterCard is StringParameterCard)
            {
                node = CreateNode(ENodeType.StringValueNode, position);
            }
            else
            {
                Debug.LogError("[AnimationGraph][GraphView]: Unknown Parameter Type, " + parameterCard);
                node = CreateNode(ENodeType.BoolValueNode, position);
            }

            var valueNode = node as ValueGraphNode;
            valueNode.InitializeDefault();
            valueNode.CombineWithParameter(parameterCard);
            
            return node;
        }

        private GraphNode CreateNodeFromAsset(NodeData data)
        {
            var node = CreateNode(data.nodeType, new Vector2(data.positionX, data.positionY));
            node.LoadNodeData(data);
            return node;
        }

        private GraphNode GetNodeById(int id)
        {
            GraphNode result = null;
            nodes.ForEach(node =>
            {
                var graphNode = node as GraphNode;
                if (graphNode != null && graphNode.id == id)
                {
                    result = graphNode;
                }
            });
            return result;
        }
        
        private NodePort GetPortById(int id)
        {
            NodePort result = null;
            ports.ForEach(port =>
            {
                var nodePort = port as NodePort;
                if (nodePort != null && nodePort.id == id)
                {
                    result = nodePort;
                }
            });
            return result;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList();
        }

        public void Compile(AnimationGraph compiledGraph)
        {
            compiledGraph.nodes = new List<NodeConfig>();
            compiledGraph.nodeConnections = new List<Connection>();
            nodes.ForEach(node =>
            {
                GraphNode graphNode = node as GraphNode;
                if (graphNode != null)
                {
                    compiledGraph.nodes.Add(graphNode.nodeConfig);
                    if (graphNode.nodeType == ENodeType.FinalPoseNode)
                    {
                        compiledGraph.finalPosePoseNode = graphNode.nodeConfig as FinalPosePoseNodeConfig;
                    }
                }
            });
            
            edges.ForEach(edge =>
            {
                var inputPort = edge.input as NodePort;
                var outputPort = edge.output as NodePort;
                Connection connection = new Connection();
                connection.sourceNodeId = outputPort.GraphNode.nodeConfig.id;
                connection.targetNodeId = inputPort.GraphNode.nodeConfig.id;
                connection.targetSlotIndex = inputPort.portIndex;
                compiledGraph.nodeConnections.Add(connection);
            });
        }

        public void Save()
        {
            m_AnimationGraphAsset.nodes = new List<NodeData>();
            m_AnimationGraphAsset.ports = new List<PortData>();
            m_AnimationGraphAsset.edges = new List<EdgeData>();
            
            nodes.ForEach(node =>
            {
                GraphNode graphNode = node as GraphNode;
                if (graphNode != null)
                {
                    NodeData nodeData = new NodeData();
                    nodeData.id = graphNode.id;
                    nodeData.nodeType = graphNode.nodeType;
                    nodeData.positionX = graphNode.GetPosition().x;
                    nodeData.positionY = graphNode.GetPosition().y;
                    nodeData.nodeConfig = graphNode.nodeConfig;
                    m_AnimationGraphAsset.nodes.Add(nodeData);
                }
            });

            ports.ForEach(port =>
            {
                NodePort nodePort = port as NodePort;
                if (nodePort != null)
                {
                    PortData portData = new PortData();
                    switch (port.direction)
                    {
                        case Direction.Input:
                            portData.direction = EPortDirection.Input;
                            break;
                        case Direction.Output:
                            portData.direction = EPortDirection.Output;
                            break;
                    }
                    
                    switch (port.capacity)
                    {
                        case Port.Capacity.Single:
                            portData.capacity = EPortCapacity.Single;
                            break;
                        case Port.Capacity.Multi:
                            portData.capacity = EPortCapacity.Multi;
                            break;
                    }

                    portData.nodeId = nodePort.GraphNode.id;
                    portData.portId = nodePort.id;
                    portData.portName = nodePort.portName;
                    portData.portIndex = nodePort.portIndex;
                    m_AnimationGraphAsset.ports.Add(portData);
                }
            });

            edges.ForEach(edge =>
            {
                EdgeData edgeData = new EdgeData();
                var inputPort = edge.input as NodePort;
                var outputPort = edge.output as NodePort;
                if (inputPort != null && outputPort != null)
                {
                    edgeData.inputPort = inputPort.id;
                    edgeData.outputPort = outputPort.id;
                    m_AnimationGraphAsset.edges.Add(edgeData);
                }
            });

            EditorUtility.SetDirty(m_AnimationGraphAsset);
            AssetDatabase.SaveAssets();
        }

        public void ClearAnimationGraphView()
        {
            DeleteElements(nodes.ToList());
            DeleteElements(edges.ToList());
            DeleteElements(ports.ToList());
        }

        public void LoadAnimGraphAsset(AnimationGraphAsset graphAsset)
        {
            m_AnimationGraphAsset = graphAsset;
            LoadNodes();
            LoadPorts();
            LoadEdges();
        }

        private void LoadNodes()
        {
            foreach (var nodeData in m_AnimationGraphAsset.nodes)
            {
                var graphNode = CreateNodeFromAsset(nodeData);
            }
        }

        private void LoadPorts()
        {
            foreach (var portData in m_AnimationGraphAsset.ports)
            {
                var node = GetNodeById(portData.nodeId);
                
                Direction direction = Direction.Input;
                Port.Capacity capacity = Port.Capacity.Single;
                switch (portData.direction)
                {
                    case EPortDirection.Input:
                        direction = Direction.Input;
                        break;
                    case EPortDirection.Output:
                        direction = Direction.Output;
                        break;
                }
                    
                switch (portData.capacity)
                {
                    case EPortCapacity.Single:
                        capacity = Port.Capacity.Single;
                        break;
                    case EPortCapacity.Multi:
                        capacity = Port.Capacity.Multi;
                        break;
                }
                node.CreatePort(direction, capacity, portData.portName, portData.portIndex, portData.portId);
            }
        }

        private void LoadEdges()
        {
            foreach (var edgeData in m_AnimationGraphAsset.edges)
            {
                NodePort inputPort = GetPortById(edgeData.inputPort);
                NodePort outputPort = GetPortById(edgeData.outputPort);
                Edge edge = new Edge();
                edge.input = inputPort;
                edge.output = outputPort;
                inputPort.Connect(edge);
                outputPort.Connect(edge);
                AddElement(edge);
            }
        }
    }
}
