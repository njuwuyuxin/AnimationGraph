using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class FinalPoseNodeConfig : NodeConfig
    {
        public override IAnimationGraphNodeInterface GenerateAnimationGraphNode(AnimationGraphRuntime graphRuntime)
        {
            FinalPoseNode finalPoseNode = new FinalPoseNode();
            finalPoseNode.m_NodeConfig = this;
            finalPoseNode.InitializeGraphNode(graphRuntime);
            return finalPoseNode;
        }
    }
}
