
using System;

namespace AnimationGraph
{
    [Serializable]
    public abstract class NodeConfig
    {
        public int id;

        public abstract IAnimationGraphNodeInterface GenerateAnimationGraphNode(AnimationGraphRuntime graphRuntime);
    }
}
