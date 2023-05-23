using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class StateMachineNode : PoseNode<StateMachinePoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;
        
        //TODO: Need to Define a state machine graph data structure

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            id = m_NodeConfig.id;
            SetPoseInputSlotCount(m_NodeConfig.states.Count);
            SetValueInputSlotCount(0);
            
            //TODO: deserialize nodeconfig to a runtime state machine graph
            
            m_AnimationGraphRuntime = animationGraphRuntime;
        }

        public override void OnStart()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnDisconnected()
        {
        }

        // public override Playable GetPlayable()
        // {
        //     return m_AnimationMixerPlayable;
        // }
    }
}