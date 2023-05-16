using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public abstract class SelectorNode<TNodeConfig> : PoseNode<TNodeConfig> where TNodeConfig :PoseNodeConfig
    {
        protected Playable m_OldPlayable;
        protected Playable m_CurrentActivePlayable;
        protected AnimationMixerPlayable m_MixerPlayable;
        protected float m_TransitionTimer = 0f;
        protected float m_TransitionTime = 0.25f;
        
        private bool m_IsTransitioning;

        protected void InitializePlayable(AnimationGraphRuntime animationGraphRuntime)
        {
            //Input 0 = old playable, Input 1 = new playable
            m_MixerPlayable = AnimationMixerPlayable.Create(animationGraphRuntime.m_PlayableGraph, 2);
            m_OldPlayable = Playable.Null;
        }
        
        protected void StartTransition()
        {
            m_MixerPlayable.DisconnectInput(0);
            m_MixerPlayable.DisconnectInput(1);
            m_MixerPlayable.ConnectInput(0, m_OldPlayable, 0);
            m_MixerPlayable.ConnectInput(1, m_CurrentActivePlayable, 0);
            
            if (m_OldPlayable.IsNull())
            {
                m_MixerPlayable.SetInputWeight(0, 0);
                m_MixerPlayable.SetInputWeight(1, 1);
            }
            else
            {
                m_MixerPlayable.SetInputWeight(0, 1);
                m_MixerPlayable.SetInputWeight(1, 0);
                m_IsTransitioning = true;
                m_TransitionTimer = 0f;
            }
            
            m_OldPlayable = m_CurrentActivePlayable;
        }
        
        protected void UpdateTransition(float deltaTime)
        {
            if (m_IsTransitioning)
            {
                m_TransitionTimer += deltaTime;
                if (m_TransitionTimer >= m_TransitionTime)
                {
                    m_IsTransitioning = false;
                    m_MixerPlayable.SetInputWeight(0, 0);
                    m_MixerPlayable.SetInputWeight(1, 1);
                }

                float transitionPercentage = m_TransitionTimer / m_TransitionTime;
                m_MixerPlayable.SetInputWeight(0, 1 - transitionPercentage);
                m_MixerPlayable.SetInputWeight(1, transitionPercentage);
            }
        }
        
        public override Playable GetPlayable()
        {
            return m_MixerPlayable;
        }
    }
}
