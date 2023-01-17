using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class ValueGraphNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.BoolValueNode;
        protected ParameterCard m_ParameterCard;

        public ValueGraphNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            
        }

        public void CombineWithParameter(ParameterCard parameterCard)
        {
            m_ParameterCard = parameterCard;
            var boolValueNodeConfig = (ValueNodeConfig) m_NodeConfig;
            boolValueNodeConfig.parameterId = parameterCard.id;
            boolValueNodeConfig.parameterName = parameterCard.parameterName;
        }
    }
}
