using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class IntValueGraphNode : ValueGraphNode
    {
        public override ENodeType nodeType => ENodeType.IntValueNode;
        private ParameterCard m_ParameterCard;

        public IntValueGraphNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "IntValue";
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new IntValueNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", 0);
        }
    }
}
