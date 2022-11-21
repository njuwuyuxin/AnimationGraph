
namespace AnimationGraph
{
    public class BoolValueNode : ValueNode<BoolValueNodeConfig>
    {
        private bool m_Value;
        public override bool boolValue => m_Value;

        public override void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime)
        {
            m_Value = m_NodeConfig.value;
        }
        
    }
}
