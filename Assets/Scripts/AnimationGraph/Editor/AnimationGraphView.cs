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

        public AnimationGraphView()
        {
            AddGridBackground();
            AddManipulators();
            AddStyleSheet();
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
        }

        private IManipulator CreateContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(
                    "Add Node", actionEvent => AddElement(CreateNode(actionEvent.eventInfo.mousePosition))
                    )
                );
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

        private Node CreateNode(Vector2 position)
        {
            var node = new GraphNode(this, position);
            node.Initialize();
            return node;
        }

        private Node CreateNodeFromAsset(NodeData data)
        {
            var node = new GraphNode(this, new Vector2(data.positionX,data.positionY));
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
                    nodeData.nodeType = ENodeType.BaseNode;
                    nodeData.positionX = graphNode.GetPosition().x;
                    nodeData.positionY = graphNode.GetPosition().y;
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
                    portData.portName = port.portName;
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
            EditorUtility.DisplayDialog("Success", "Animation Graph Save Successfully!", "OK");
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
                AddElement(graphNode);
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
                node.CreatePort(direction, capacity, portData.portName, portData.portId);
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
