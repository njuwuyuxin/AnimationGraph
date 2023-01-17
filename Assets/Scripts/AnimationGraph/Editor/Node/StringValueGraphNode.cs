using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class StringValueGraphNode : ValueGraphNode
    {
        public override ENodeType nodeType => ENodeType.StringValueNode;
        private ParameterCard m_ParameterCard;

        public StringValueGraphNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "StringValue";
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new StringValueNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", 0);
        }
    }
}
