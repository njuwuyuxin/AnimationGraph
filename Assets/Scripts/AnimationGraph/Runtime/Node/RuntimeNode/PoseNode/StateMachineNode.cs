using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class StateMachineNode : PoseNode<StateMachinePoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;
        private AnimationMixerPlayable m_AnimationMixerPlayable;
        
        //TODO: Need to Define a state machine graph data structure
        private class State
        {
            public int portIndex;
            public StatePoseNodeConfig config;
            public List<TransitionConfig> transitions = new List<TransitionConfig>();
        }

        private Dictionary<int, State> m_States;
        private State m_CurrentState;

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            m_AnimationGraphRuntime = animationGraphRuntime;
            id = m_NodeConfig.id;
            SetPoseInputSlotCount(m_NodeConfig.states.Count);
            SetValueInputSlotCount(0);

            m_AnimationMixerPlayable = AnimationMixerPlayable.Create(m_AnimationGraphRuntime.m_PlayableGraph, 1);
            
            //TODO: deserialize nodeconfig to a runtime state machine graph
            
            m_States = new Dictionary<int, State>();
            for(int i=0;i<m_NodeConfig.states.Count;i++)
            {
                var state = m_NodeConfig.states[i];
                m_States.Add(state.id, new State() { portIndex = i, config = state });
            }

            foreach (var transition in m_NodeConfig.transitions)
            {
                if (m_States.TryGetValue(transition.sourceStateId, out var state))
                {
                    state.transitions.Add(transition);
                }
            }
        }

        public override void OnStart()
        {
            m_CurrentState = m_States[m_NodeConfig.defaultStateId];
            var playable = GetStateNode(m_CurrentState).GetPlayable();
            m_AnimationMixerPlayable.ConnectInput(0, playable, 0);
            m_AnimationMixerPlayable.SetInputWeight(0, 1);
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var transition in m_CurrentState.transitions)
            {
                if (ValidateConditions(transition.conditions))
                {
                    StartTransition(m_CurrentState, m_States[transition.targetStateId], transition);
                }
            }
        }

        private bool ValidateConditions(List<TransitionCondition> conditions)
        {
            foreach (var condition in conditions)
            {
                GraphParameter parameter = m_AnimationGraphRuntime.GetParameterById(condition.parameterId);
                switch (condition.conditionType)
                {
                    case EConditionType.NotEqual:
                        if (parameter.value.IsEqual(condition.value))
                        {
                            return false;
                        }
                        break;
                    case EConditionType.Equal:
                        if (!parameter.value.IsEqual(condition.value))
                        {
                            return false;
                        }
                        break;
                    case EConditionType.Greater:
                        if (!parameter.value.IsGreaterThan(condition.value))
                        {
                            return false;
                        }
                        break;
                    case EConditionType.GreaterEqual:
                        if (!parameter.value.IsGreaterEqualThan(condition.value))
                        {
                            return false;
                        }
                        break;
                    case EConditionType.Less:
                        if (!parameter.value.IsLessThan(condition.value))
                        {
                            return false;
                        }
                        break;
                    case EConditionType.LessEqual:
                        if (!parameter.value.IsLessEqualThan(condition.value))
                        {
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        private void StartTransition(State oldState, State newState, TransitionConfig transitionConfig)
        {
            m_AnimationMixerPlayable.DisconnectInput(0);
            m_AnimationMixerPlayable.ConnectInput(0, GetStateNode(newState).GetPlayable(), 0);
            m_AnimationMixerPlayable.SetInputWeight(0, 1);
            m_CurrentState = newState;
        }

        private IPoseNodeInterface GetStateNode(State state)
        {
            return m_InputPoseNodes[state.portIndex];
        }
        
        public override void OnDisconnected()
        {
        }

        public override Playable GetPlayable()
        {
            return m_AnimationMixerPlayable;
        }
    }
}