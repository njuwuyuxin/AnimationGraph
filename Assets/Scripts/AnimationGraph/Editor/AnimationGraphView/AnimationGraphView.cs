using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public partial class AnimationGraphView : GraphView
    {
        private const string k_StyleSheetPrefix = "Assets/Scripts/AnimationGraph/Editor/StyleSheet/";
        private AnimationGraphAsset m_AnimationGraphAsset;
        public NodeInspector inspector => m_Inspector;
        public ParameterBoard parameterBoard => m_ParameterBoard;
        private ParameterBoard m_ParameterBoard;
        private NodeInspector m_Inspector;

        public AnimationGraphView(ParameterBoard parameterBoard, NodeInspector inspector)
        {
            m_ParameterBoard = parameterBoard;
            m_Inspector = inspector;
            AddGridBackground();
            AddManipulators();
            AddStyleSheet();
            RegisterCallbacks();
            graphViewChanged += OnGraphViewChanged;
        }

        public GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null && graphViewChange.elementsToRemove.Count > 0)
            {
                foreach (var element in graphViewChange.elementsToRemove)
                {
                    GraphNode graphNode = element as GraphNode;
                    if (graphNode != null)
                    {
                        graphNode.OnDestroy();
                    }
                }
            }
            
            return graphViewChange;
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
                    menuEvent.menu.AppendAction(
                        "Add BoolSelector Node",
                        actionEvent => CreateDefaultNode(ENodeType.BoolSelectorNode, MouseToViewPosition(actionEvent.eventInfo.mousePosition))
                    );
                    menuEvent.menu.AppendAction(
                        "Add StringSelector Node",
                        actionEvent => CreateDefaultNode(ENodeType.StringSelectorNode, MouseToViewPosition(actionEvent.eventInfo.mousePosition))
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
        private Vector2 MouseToViewPosition(Vector2 mousePosition)
        {
            Vector2 graphViewPosition = VisualElementExtensions.LocalToWorld(this, new Vector2(transform.position.x,transform.position.y));
            return mousePosition - graphViewPosition;
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

        public void ClearAnimationGraphView()
        {
            DeleteElements(nodes.ToList());
            DeleteElements(edges.ToList());
            DeleteElements(ports.ToList());
        }

        public void OnDestory()
        {
            graphViewChanged -= OnGraphViewChanged;
        }
    }
}
