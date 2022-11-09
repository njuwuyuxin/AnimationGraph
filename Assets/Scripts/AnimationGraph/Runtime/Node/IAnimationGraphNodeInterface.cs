using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public abstract class IAnimationGraphNodeInterface
    {
        protected List<IAnimationGraphNodeInterface> m_InputNodes = new List<IAnimationGraphNodeInterface>();
        protected List<IAnimationGraphNodeInterface> m_OutputNodes = new List<IAnimationGraphNodeInterface>();

        public abstract void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime);

        public virtual Playable GetPlayable()
        {
            throw new NotImplementedException();
        }

        public virtual void AddInputNodes(IAnimationGraphNodeInterface inputNode)
        {
            m_InputNodes.Add(inputNode);
        }

        public virtual void AddOutputNodes(IAnimationGraphNodeInterface outputNode)
        {
            m_OutputNodes.Add(outputNode);
        }

    }
}
