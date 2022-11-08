
using System;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public abstract class IAnimationGraphNodeInterface
    {
        public abstract void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime);
        
        public virtual Playable GetPlayable()
        {
            throw new NotImplementedException();
        }
    }
}
