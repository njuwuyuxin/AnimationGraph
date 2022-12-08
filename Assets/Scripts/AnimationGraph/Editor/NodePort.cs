using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class NodePort : Port
    {
        public int id { get; set; }
        public GraphNode GraphNode => m_GraphNode;
        private GraphNode m_GraphNode;

        public NodePort(GraphNode node, Orientation portOrientation, Direction portDirection, Port.Capacity portCapacity, System.Type type):base(
            portOrientation,portDirection,portCapacity,type)
        {
            EdgeConnectorListener connectorListener = new EdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<Edge>(connectorListener);
            this.AddManipulator(edgeConnector);

            m_GraphNode = node;
            id = Animator.StringToHash(Guid.NewGuid().ToString());
        }
        
        public NodePort(GraphNode node, Orientation portOrientation, Direction portDirection, Port.Capacity portCapacity, System.Type type, int id):base(
            portOrientation,portDirection,portCapacity,type)
        {
            EdgeConnectorListener connectorListener = new EdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<Edge>(connectorListener);
            this.AddManipulator(edgeConnector);

            m_GraphNode = node;
            this.id = id;
        }
    }
}