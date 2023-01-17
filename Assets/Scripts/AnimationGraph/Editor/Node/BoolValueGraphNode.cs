using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class BoolValueGraphNode : ValueGraphNode
    {
        public override ENodeType nodeType => ENodeType.BoolValueNode;
        private ParameterCard m_ParameterCard;

        public BoolValueGraphNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "BoolValue";
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new BoolValueNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", 0);
        }
    }
}
