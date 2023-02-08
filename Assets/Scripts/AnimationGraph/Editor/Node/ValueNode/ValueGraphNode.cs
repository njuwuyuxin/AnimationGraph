using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class ValueGraphNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.BoolValueNode;
        protected ParameterCard m_ParameterCard;

        public ValueGraphNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            var divider = topContainer.Q("divider");
            topContainer.Remove(divider);
            inputContainer.style.flexGrow = 0;
        }

        public void CombineWithParameter(ParameterCard parameterCard)
        {
            m_ParameterCard = parameterCard;
            var boolValueNodeConfig = (ValueNodeConfig) m_NodeConfig;
            boolValueNodeConfig.parameterId = parameterCard.id;
            boolValueNodeConfig.parameterName = parameterCard.parameterName;
        }

        public override void LoadNodeData(NodeData data)
        {
            base.LoadNodeData(data);
            ValueNodeConfig nodeConfig = data.nodeConfig as ValueNodeConfig;
            m_ParameterCard = m_AnimationGraphView.parameterBoard.TryGetParameterById(nodeConfig.parameterId);
            nodeName = nodeConfig.parameterName;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            m_ParameterCard.associatedNodes.Remove(this.id);
        }
    }
}
