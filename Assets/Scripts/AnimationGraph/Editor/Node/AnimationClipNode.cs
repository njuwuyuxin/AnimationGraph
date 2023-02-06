using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class AnimationClipNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.AnimationClipNode;

        public AnimationClipNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "AnimationClip";
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new AnimationClipPoseNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.PosePort, 0);
        }
    }
}
