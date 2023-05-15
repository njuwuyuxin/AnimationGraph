using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class StringSelectorNode : PoseNode<StringSelectorPoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;

        public Dictionary<string, int> string2PortIndex = new Dictionary<string, int>();
        public IValueNodeInterface condition => m_InputValueNodes[0];

        private Playable m_OldPlayable;
        private Playable m_CurrentActivePlayable;
        private AnimationMixerPlayable m_MixerPlayable;
        private string m_CurrentCondition;

        private bool m_IsTransitioning;
        private float m_TransitionTimer = 0f;
        private float m_TransitionTime = 0.25f;

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            id = m_NodeConfig.id;
            var config = m_NodeConfig as StringSelectorPoseNodeConfig;
            for (int i = 0; i < config.selections.Count; i++)
            {
                string2PortIndex.Add(config.selections[i], i);
            }

            SetPoseInputSlotCount(config.selections.Count);
            SetValueInputSlotCount(1);
            m_AnimationGraphRuntime = animationGraphRuntime;
            
            //Input 0 = old playable, Input 1 = new playable
            m_MixerPlayable = AnimationMixerPlayable.Create(m_AnimationGraphRuntime.m_PlayableGraph, 2);
            m_OldPlayable = Playable.Null;
        }

        public override Playable GetPlayable()
        {
            return m_MixerPlayable;
        }

        public override void OnStart()
        {
            ChangeSourcePlayable();
            m_CurrentCondition = condition.stringValue;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (condition.stringValue != m_CurrentCondition)
            {
                ChangeSourcePlayable();
                m_CurrentCondition = condition.stringValue;
            }

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

        private void ChangeSourcePlayable()
        {
            if (string2PortIndex.TryGetValue(condition.stringValue, out int portIndex))
            {
                var node = m_InputPoseNodes[portIndex];
                node.OnStart();
                m_CurrentActivePlayable = node.GetPlayable();
                m_MixerPlayable.DisconnectInput(0);
                m_MixerPlayable.DisconnectInput(1);
                
                m_MixerPlayable.ConnectInput(0, m_OldPlayable, 0);
                m_MixerPlayable.ConnectInput(1, m_CurrentActivePlayable, 0);
                m_MixerPlayable.SetInputWeight(0, 1);
                m_MixerPlayable.SetInputWeight(1, 0);
                m_OldPlayable = m_CurrentActivePlayable;
                
                m_IsTransitioning = true;
                m_TransitionTimer = 0f;
            }
            else
            {
                Debug.LogError("StringSelectorNode: No String matcheds name: " + condition.stringValue);
            }
        }
    }
}