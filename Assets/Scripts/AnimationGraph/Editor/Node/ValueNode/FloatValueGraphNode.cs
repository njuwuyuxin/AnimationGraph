using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class FloatValueGraphNode : ValueGraphNode
    {
        public override ENodeType nodeType => ENodeType.FloatValueNode;
        private ParameterCard m_ParameterCard;

        public FloatValueGraphNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "FloatValue";
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new FloatValueNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.ValuePort, 0);
        }
    }
}
