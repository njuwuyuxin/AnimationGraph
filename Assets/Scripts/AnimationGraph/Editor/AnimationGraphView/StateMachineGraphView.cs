using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StateMachineGraphView : GraphView
    {
        private const string k_StyleSheetPrefix = "Assets/Scripts/AnimationGraph/Editor/StyleSheet/";
        public NodeInspector inspector => m_Inspector;
        public ParameterBoard parameterBoard => m_ParameterBoard;
        private ParameterBoard m_ParameterBoard;
        private NodeInspector m_Inspector;
        private VisualElement m_Container;
        private AnimationGraphView m_OwnerGraph;
        private StateMachineNode m_StateMachineNode;

        public StateNode currentSelectedNode { get; set; }
        public StateNode lastSelectedNode { get; set; }
        public bool isMakingTransition { get; set; }
        
        public class TransitionToAdd
        {
            public StateNode source;
            public StateNode target;
        }
        
        public TransitionToAdd transitionToAdd { get; set; }

        public StateMachineGraphView(VisualElement container, AnimationGraphView ownerGraph, StateMachineNode stateMachineNode, ParameterBoard parameterBoard, NodeInspector inspector)
        {
            m_Container = container;
            m_OwnerGraph = ownerGraph;
            m_StateMachineNode = stateMachineNode;
            m_ParameterBoard = parameterBoard;
            m_Inspector = inspector;

            RegisterCallback<MouseDownEvent>(OnMouseDown);
            AddGridBackground();
            AddManipulators();
            AddStyleSheet();
            graphViewChanged += OnGraphViewChanged;
            
            Button returnButton = new Button(ReturnBack);
            returnButton.Add(new Label("Return back"));
            this.Add(returnButton);

            transitionToAdd = new TransitionToAdd();
        }
        
        private void ReturnBack()
        {
            m_OwnerGraph.CloseStateMachineGraphView();
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
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            this.AddManipulator(CreateContextualMenu());
        }
        
        private IManipulator CreateContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(OnContextMenuPopulate);
            return contextualMenuManipulator;
        }
        
        private void OnContextMenuPopulate(ContextualMenuPopulateEvent menuEvent)
        {
            DropdownMenuAction.Status makeTransitionStatus = currentSelectedNode == null
                ? DropdownMenuAction.Status.Hidden
                : DropdownMenuAction.Status.Normal;

            menuEvent.menu.AppendAction(
                "Add State",
                actionEvent => AddState(MouseToViewPosition(actionEvent.eventInfo.mousePosition))
            );
            
            menuEvent.menu.AppendAction(
                "Make Transition",
                actionEvent => StartMakingTransition(),makeTransitionStatus
            );
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (isMakingTransition && evt.target == this)
            {
                isMakingTransition = false;
            }
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

        private void AddState(Vector2 position)
        {
            var stateNode = new StateNode(this.m_OwnerGraph, this, position);
            stateNode.InitializeDefault();
            AddElement(stateNode);
        }

        private void StartMakingTransition()
        {
            isMakingTransition = true;
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

        public void TryCreateTransition()
        {
            if (transitionToAdd.source == null || transitionToAdd.target == null)
            {
                return;
            }

            if (transitionToAdd.source == transitionToAdd.target)
            {
                return;
            }

            var transition = new StateTransition(this, transitionToAdd.source, transitionToAdd.target);
            AddElement(transition);
            transitionToAdd.source = null;
            transitionToAdd.target = null;
        }
    }
}
