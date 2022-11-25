using UnityEngine.Playables;

namespace AnimationGraph
{
    public class BoolSelectorNode : PoseNode<BoolSelectorPoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;
        public IPoseNodeInterface trueNode => m_InputPoseNodes[0];
        public IPoseNodeInterface falseNode => m_InputPoseNodes[1];
        public IValueNodeInterface condition => m_InputValueNodes[0];

        private Playable m_CurrentActivePlayable;
        private bool m_CurrentCondition;

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            id = m_NodeConfig.id;
            SetPoseInputSlotCount(2);
            SetValueInputSlotCount(1);
            m_AnimationGraphRuntime = animationGraphRuntime;
        }

        public override Playable GetPlayable()
        {
            return m_CurrentActivePlayable;
        }

        public override void OnStart()
        {
            ChangeSourcePlayable();
            m_CurrentCondition = condition.boolValue;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (condition.boolValue != m_CurrentCondition)
            {
                ChangeSourcePlayable();
                m_CurrentCondition = condition.boolValue;
            }
        }

        private void ChangeSourcePlayable()
        {
            if (condition.boolValue)
            {
                trueNode.OnStart();
                m_CurrentActivePlayable = trueNode.GetPlayable();
            }
            else
            {
                falseNode.OnStart();
                m_CurrentActivePlayable = falseNode.GetPlayable();
            }
        }
    }
}