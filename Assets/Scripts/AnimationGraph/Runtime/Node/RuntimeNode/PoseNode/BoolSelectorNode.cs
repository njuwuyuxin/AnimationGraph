using UnityEngine.Playables;

namespace AnimationGraph
{
    public class BoolSelectorNode : PoseNode<BoolSelectorPoseNodeConfig>
    {
        private AnimationGraphRuntime m_AnimationGraphRuntime;
        public IPoseNodeInterface trueNode => m_InputPoseNodes[0];
        public IPoseNodeInterface falseNode => m_InputPoseNodes[1];
        public IValueNodeInterface condition => m_InputValueNodes[0];

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            SetPoseInputSlotCount(2);
            SetValueInputSlotCount(1);
            m_AnimationGraphRuntime = animationGraphRuntime;
        }

        public override Playable GetPlayable()
        {
            if (condition.boolValue)
            {
                return trueNode.GetPlayable();
            }
            else
            {
                return falseNode.GetPlayable();
            }
        }
        
    }
}