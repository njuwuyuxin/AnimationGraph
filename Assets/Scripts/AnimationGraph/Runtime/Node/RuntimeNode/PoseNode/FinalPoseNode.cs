using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class FinalPoseNode : PoseNode<FinalPosePoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            SetPoseInputSlotCount(1);
            m_AnimationGraphRuntime = animationGraphRuntime;
        }

        public override Playable GetPlayable()
        {
            if (m_InputPoseNodes.Length == 0)
            {
                Debug.LogError("FinalPose has no input!");
                throw new NotImplementedException();
            }
            return m_InputPoseNodes[0].GetPlayable();
        }
    }
}