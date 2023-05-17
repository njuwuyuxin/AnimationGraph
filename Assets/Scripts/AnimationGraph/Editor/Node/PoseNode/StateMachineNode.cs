using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StateMachineNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.StateMachineNode;

        public StateMachineNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "StateMachine";
            ColorUtility.TryParseHtmlString("#663366", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new Blend1DPoseNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.PosePort, 0);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.clickCount == 2)
            {
                OpenStateMachineGraphView();
            }
        }

        private void OpenStateMachineGraphView()
        {
            Debug.Log("Open State Machine Graph View");
            m_AnimationGraphView.OpenStateMachineGraphView(this);
        }
    }
}
