using System;

namespace AnimationGraph
{
    [Serializable]
    public class BoolSelectorPoseNodeConfig : PoseNodeConfig
    {
        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            BoolSelectorNode boolSelectorNode = new BoolSelectorNode();
            boolSelectorNode.m_NodeConfig = this;
            boolSelectorNode.InitializeGraphNode(graphRuntime);
            return boolSelectorNode;
        }
    }
}