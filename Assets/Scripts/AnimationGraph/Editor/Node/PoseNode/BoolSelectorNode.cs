using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class BoolSelectorNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.BoolSelectorNode;

        public BoolSelectorNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "BoolSelector";
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new BoolSelectorPoseNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.PosePort, 0);
            CreatePort(Direction.Input, Port.Capacity.Multi, "True", NodePort.EPortType.PosePort, 0);
            CreatePort(Direction.Input, Port.Capacity.Multi, "False", NodePort.EPortType.PosePort, 1);
            CreatePort(Direction.Input, Port.Capacity.Multi, "Condition", NodePort.EPortType.ValuePort, 0);
        }
    }
}
