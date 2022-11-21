using System.Collections.Generic;

namespace AnimationGraph
{
    public abstract class IValueNodeInterface : INode
    {
        public override ENodeType nodeType => ENodeType.ValueNode;

        public virtual bool boolValue { get; }
        public virtual float floatValue { get; }
        public virtual string stringValue { get; }
        
        protected List<IPoseNodeInterface> m_OutputPoseNodes = new List<IPoseNodeInterface>();
        
        public override void AddOutputNode(INode outputNode)
        {
            AddOutputPoseNode(outputNode as IPoseNodeInterface);
        }
        
        protected virtual void AddOutputPoseNode(IPoseNodeInterface outputNode)
        {
            m_OutputPoseNodes.Add(outputNode);
        }
    }
}
