using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class StringSelectorNode : PoseNode<StringSelectorPoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;

        public Dictionary<string, int> string2PortIndex = new Dictionary<string, int>();
        public IValueNodeInterface condition => m_InputValueNodes[0];

        private Playable m_CurrentActivePlayable;
        private string m_CurrentCondition;

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            id = m_NodeConfig.id;
            SetPoseInputSlotCount(2);
            SetValueInputSlotCount(1);
            m_AnimationGraphRuntime = animationGraphRuntime;
            var config = m_NodeConfig as StringSelectorPoseNodeConfig;
            for (int i = 0; i < config.selections.Count; i++)
            {
                string2PortIndex.Add(config.selections[i], i);
            }
        }

        public override Playable GetPlayable()
        {
            return m_CurrentActivePlayable;
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
        }

        private void ChangeSourcePlayable()
        {
            if (string2PortIndex.TryGetValue(condition.stringValue, out int portIndex))
            {
                var node = m_InputPoseNodes[portIndex];
                node.OnStart();
                m_CurrentActivePlayable = node.GetPlayable();
            }
            else
            {
                Debug.LogError("StringSelectorNode: No String matcheds name: " + condition.stringValue);
            }
        }
    }
}