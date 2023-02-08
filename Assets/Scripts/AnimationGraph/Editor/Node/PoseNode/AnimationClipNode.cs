using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class AnimationClipNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.AnimationClipNode;

        public AnimationClipNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            nodeName = "AnimationClip (null)";
            m_NodeConfig = new AnimationClipPoseNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.PosePort, 0);
        }

        public override void LoadNodeData(NodeData data)
        {
            base.LoadNodeData(data);
            var animationClip = (data.nodeConfig as AnimationClipPoseNodeConfig).clip;
            if (animationClip != null)
            {
                nodeName = animationClip.name;
            }
            else
            {
                nodeName = "AnimationClip (null)";
            }
        }

        public override void OnNodeConfigUpdate()
        {
            base.OnNodeConfigUpdate();
            var animationClip = (nodeConfig as AnimationClipPoseNodeConfig).clip;
            if (animationClip != null)
            {
                nodeName = animationClip.name;
            }
            else
            {
                nodeName = "AnimationClip (null)";
            }
        }
    }
}
