using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StateMachineGraphView : GraphViewBase
    {
        private const string k_StyleSheetPrefix = "Assets/Scripts/AnimationGraph/Editor/StyleSheet/";
        public AnimationGraphInspector inspector => m_Inspector;
        public ParameterBoard parameterBoard => m_ParameterBoard;
        private ParameterBoard m_ParameterBoard;
        private AnimationGraphInspector m_Inspector;
        private VisualElement m_Container;
        private AnimationGraphView m_AnimationGraphView;
        private StateMachineNode m_StateMachineNode;
        public StateMachineNode stateMachineNode => m_StateMachineNode;

        private StateTransition m_PreviewTransition;

        public StateNode currentSelectedNode { get; set; }
        public StateNode lastSelectedNode { get; set; }

        private bool m_IsMakingTransition;

        public bool isMakingTransition
        {
            get
            {
                return m_IsMakingTransition;
            }
            set
            {
                m_IsMakingTransition = value;
                if (!m_IsMakingTransition && m_PreviewTransition != null)
                {
                    RemoveElement(m_PreviewTransition);
                    m_PreviewTransition = null;
                }
            }
        }

        public class TransitionToAdd
        {
            public StateNode source;
            public StateNode target;
        }
        
        public TransitionToAdd transitionToAdd { get; set; }

        public StateMachineGraphView(VisualElement container, AnimationGraphView animationGraphView,
            StateMachineNode stateMachineNode, ParameterBoard parameterBoard, AnimationGraphInspector inspector)
        {
            m_Container = container;
            m_AnimationGraphView = animationGraphView;
            m_StateMachineNode = stateMachineNode;
            m_ParameterBoard = parameterBoard;
            m_Inspector = inspector;

            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            AddDefaultManipulators();
            this.AddManipulator(CreateContextualMenu());
            AddStyleSheet();

            Button returnButton = new Button(ReturnBack);
            returnButton.Add(new Label("Return back"));
            this.Add(returnButton);

            transitionToAdd = new TransitionToAdd();

            InitializeFromStateMachineNode();
        }

        private void InitializeFromStateMachineNode()
        {
            foreach (var stateConfig in m_StateMachineNode.stateConfigs)
            {
                var stateNode = new StateNode(this.m_AnimationGraphView, this, stateConfig.position);
                stateNode.LoadFromConfig(stateConfig);
                AddElement(stateNode);
            }
        }

        private void ReturnBack()
        {
            m_AnimationGraphView.CloseStateMachineGraphView();
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
                actionEvent => StartMakingTransition(actionEvent),makeTransitionStatus
            );
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (isMakingTransition && evt.target == this)
            {
                isMakingTransition = false;
            }
        }
        
        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (isMakingTransition && m_PreviewTransition != null)
            {
                m_PreviewTransition.candidatePosition = evt.mousePosition;
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

        private void AddState(Vector2 position)
        {
            var stateNode = new StateNode(this.m_AnimationGraphView, this, position);
            stateNode.InitializeDefault();
            AddElement(stateNode);

            m_StateMachineNode.OnAddState(stateNode.nodeConfig as StatePoseNodeConfig);
        }

        private void StartMakingTransition(DropdownMenuAction actionEvent)
        {
            isMakingTransition = true;
            if (m_PreviewTransition == null)
            {
                m_PreviewTransition = new StateTransition(this, currentSelectedNode, null);
                m_PreviewTransition.candidatePosition = actionEvent.eventInfo.mousePosition;
                AddElement(m_PreviewTransition);
            }
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
            transition.InitializeDefault();
            AddElement(transition);
            transitionToAdd.source = null;
            transitionToAdd.target = null;
        }
        
    }
}
