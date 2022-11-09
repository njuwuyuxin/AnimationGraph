using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class FinalPoseNode : AnimationGraphNode<FinalPoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            m_AnimationGraphRuntime = animationGraphRuntime;
        }

        public override Playable GetPlayable()
        {
            if (m_InputNodes.Count == 0)
            {
                Debug.LogError("FinalPose has no input!");
                throw new NotImplementedException();
            }
            return m_InputNodes[0].GetPlayable();
        }
    }
}