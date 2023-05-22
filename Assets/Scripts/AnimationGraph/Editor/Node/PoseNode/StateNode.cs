using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StateNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.StateNode;
        private StateMachineGraphView m_StateMachineView;

        private HashSet<StateTransition> m_InputTransitions = new HashSet<StateTransition>();
        private HashSet<StateTransition> m_OutputTransitions = new HashSet<StateTransition>();

        public HashSet<StateTransition> inputTransitions => m_InputTransitions;
        public HashSet<StateTransition> outputTransitions => m_OutputTransitions;

        public StateNode(AnimationGraphView graphView, StateMachineGraphView stateMachineView, Vector2 position) : base(graphView,position)
        {
            m_StateMachineView = stateMachineView;
            var divider = contentContainer.Q("divider");
            // divider.RemoveFromHierarchy();
            // inputContainer.style.flexGrow = 0;
            // topContainer.RemoveFromHierarchy();
            ColorUtility.TryParseHtmlString("#006633", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
            
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            nodeName = "State";
            m_NodeConfig = new StatePoseNodeConfig();
            m_NodeConfig.SetId(id);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button == 1)
            {
                m_StateMachineView.transitionToAdd.source = this;
            }
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            if (evt.button == 1)
            {
                m_StateMachineView.transitionToAdd.target = this;
                m_StateMachineView.TryCreateTransition();
                evt.StopImmediatePropagation();
            }
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            foreach (var inputTransition in m_InputTransitions)
            {
                inputTransition.to = evt.newRect.center;
                inputTransition.UpdateTransitionControl();
            }

            foreach (var outputTransition in m_OutputTransitions)
            {
                outputTransition.from = evt.newRect.center;
                outputTransition.UpdateTransitionControl();
            }
        }

        public void AddInputTransition(StateTransition transition)
        {
            m_InputTransitions.Add(transition);
        }
        
        public void AddOutputTransition(StateTransition transition)
        {
            m_OutputTransitions.Add(transition);
        }
        
    }
}
