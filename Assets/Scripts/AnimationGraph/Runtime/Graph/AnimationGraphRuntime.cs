using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{

    public class AnimationGraphRuntime
    {
        private AnimationActor m_Actor;
        private AnimationGraph m_AnimationGraph;
        private PlayableGraph m_PlayableGraph;
        private AnimationPlayableOutput m_Output;
        
        public AnimationGraphRuntime(AnimationActor actor, AnimationGraph animationGraph)
        {
            m_Actor = actor;
            m_AnimationGraph = animationGraph;
        }

        public void Initialize()
        {
            m_PlayableGraph = PlayableGraph.Create();
            m_PlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            m_Output = AnimationPlayableOutput.Create(m_PlayableGraph,
                m_Actor.gameObject.name + "_" + m_Actor.gameObject.GetInstanceID(), m_Actor.animator);

            AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(m_PlayableGraph, m_AnimationGraph.clipNode.clip);
            m_Output.SetSourcePlayable(clipPlayable);
            m_PlayableGraph.Play();
        }

        public void OnUpdate(float deltaTime)
        {
            
        }

        public void Destroy()
        {
            m_PlayableGraph.Destroy();
        }
    }
}
