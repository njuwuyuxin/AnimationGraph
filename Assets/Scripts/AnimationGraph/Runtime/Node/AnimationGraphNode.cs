
using UnityEngine.Animations;

namespace AnimationGraph
{
    public abstract class AnimationGraphNode<TNodeConfig> : IAnimationGraphNodeInterface where TNodeConfig : NodeConfig
    {
        internal TNodeConfig m_NodeConfig;

        
    }
}
