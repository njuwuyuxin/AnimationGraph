using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class FinalPoseNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.FinalPoseNode;

        public FinalPoseNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            m_NodeName = "FinalPose";
            title = m_NodeName;
        }

        public override void Initialize()
        {
            base.Initialize();
            m_NodeConfig = new FinalPosePoseNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Input, Port.Capacity.Single, "Input", 0);
        }
    }
}
