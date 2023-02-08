using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class FinalPoseNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.FinalPoseNode;

        public FinalPoseNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "FinalPose";
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new FinalPosePoseNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Input, Port.Capacity.Single, "Input", NodePort.EPortType.PosePort, 0);
        }
    }
}
