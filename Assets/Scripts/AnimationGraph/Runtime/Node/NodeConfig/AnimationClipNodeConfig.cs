using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class AnimationClipNodeConfig : NodeConfig
    {
        public AnimationClip clip;
        public float playSpeed;

        public override IAnimationGraphNodeInterface GenerateAnimationGraphNode(AnimationGraphRuntime graphRuntime)
        {
            AnimationClipNode animationClipNode = new AnimationClipNode();
            animationClipNode.m_NodeConfig = this;
            animationClipNode.InitializeGraphNode(graphRuntime);
            return animationClipNode;
        }
    }
}
