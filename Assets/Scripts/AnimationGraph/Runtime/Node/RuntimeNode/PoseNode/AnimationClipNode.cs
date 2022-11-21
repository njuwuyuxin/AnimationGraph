using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class AnimationClipNode : PoseNode<AnimationClipPoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;
        private AnimationClipPlayable m_AnimationClipPlayable;
        
        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            m_AnimationGraphRuntime = animationGraphRuntime;
            m_AnimationClipPlayable = AnimationClipPlayable.Create(m_AnimationGraphRuntime.playableGraph, m_NodeConfig.clip);
        }

        public override Playable GetPlayable()
        {
            return m_AnimationClipPlayable;
        }
    }
}