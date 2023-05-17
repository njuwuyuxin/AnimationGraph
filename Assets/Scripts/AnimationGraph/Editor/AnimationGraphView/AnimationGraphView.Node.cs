using UnityEngine;

namespace AnimationGraph.Editor
{
    public partial class AnimationGraphView
    {
        private GraphNode CreateNodeInternal(ENodeType nodeType, Vector2 position)
        {
            GraphNode node = null;
            switch (nodeType)
            {
                case ENodeType.FinalPoseNode: node = new FinalPoseNode(this, position);
                    break;
                case ENodeType.AnimationClipNode: node = new AnimationClipNode(this, position);
                    break;
                case ENodeType.BoolSelectorNode: node = new BoolSelectorNode(this, position);
                    break;
                case ENodeType.StringSelectorNode: node = new StringSelectorNode(this, position);
                    break;
                case ENodeType.BoolValueNode: node = new BoolValueGraphNode(this, position);
                    break;
                case ENodeType.IntValueNode: node = new IntValueGraphNode(this, position);
                    break;
                case ENodeType.FloatValueNode: node = new FloatValueGraphNode(this, position);
                    break;
                case ENodeType.StringValueNode: node = new StringValueGraphNode(this, position);
                    break;
                case ENodeType.Blend1DNode: node = new Blend1DNode(this, position);
                    break;
                default: node = new GraphNode(this, position);
                    break;
            }

            if (node != null)
            {
                AddElement(node);
            }
            else
            {
                Debug.LogError("[AnimationGraph][GraphView]: Create Node failed, nodeType:" + nodeType);
            }

            return node;
        }

        private GraphNode CreateDefaultNode(ENodeType nodeType, Vector2 position)
        {
            var node = CreateNodeInternal(nodeType, position);
            node.InitializeDefault();
            return node;
        }

        private GraphNode CreateParameterNode(ParameterCard parameterCard, Vector2 position)
        {
            GraphNode node = null;
            if (parameterCard is BoolParameterCard)
            {
                node = CreateNodeInternal(ENodeType.BoolValueNode, position);
            }
            else if (parameterCard is IntParameterCard)
            {
                node = CreateNodeInternal(ENodeType.IntValueNode, position);
            }
            else if (parameterCard is FloatParameterCard)
            {
                node = CreateNodeInternal(ENodeType.FloatValueNode, position);
            }
            else if (parameterCard is StringParameterCard)
            {
                node = CreateNodeInternal(ENodeType.StringValueNode, position);
            }
            else
            {
                Debug.LogError("[AnimationGraph][GraphView]: Unknown Parameter Type, " + parameterCard);
                node = CreateNodeInternal(ENodeType.BoolValueNode, position);
            }

            var valueNode = node as ValueGraphNode;
            valueNode.InitializeDefault();
            valueNode.CombineWithParameter(parameterCard);
            
            return node;
        }

        private GraphNode CreateNodeFromAsset(NodeData data)
        {
            var node = CreateNodeInternal(data.nodeType, new Vector2(data.positionX, data.positionY));
            node.LoadNodeData(data);
            return node;
        }
    }
}
