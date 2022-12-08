using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class GraphNode : Node
    {
        public int id { get; set; }

        protected AnimationGraphView m_AnimationGraphView;
        protected string m_NodeName;
        protected List<NodePort> m_InputPorts;
        protected NodePort m_OutputPort;
        
        public GraphNode(AnimationGraphView graphView, Vector2 position)
        {
            m_AnimationGraphView = graphView;
            m_NodeName = "Base Node";
            m_InputPorts = new List<NodePort>();
            SetPosition(new Rect(position,Vector2.zero));
            Draw();
        }

        public virtual void Initialize()
        {
            id = Animator.StringToHash(Guid.NewGuid().ToString());
            
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output");
            CreatePort(Direction.Input, Port.Capacity.Single, "Input1");
            CreatePort(Direction.Input, Port.Capacity.Single, "Input2");
        }

        public virtual void LoadNodeData(NodeData data)
        {
            id = data.id;
        }
        
        private void CreatePort(Direction direction, Port.Capacity capacity, string portName)
        {
            var port = new NodePort(this, Orientation.Horizontal, direction, capacity, typeof(Port));
            port.portName = portName;
            
            switch (direction)
            {
                case Direction.Input: 
                    m_InputPorts.Add(port);
                    inputContainer.Add(port);
                    break;
                case Direction.Output: 
                    m_OutputPort = port;
                    outputContainer.Add(port);
                    break;
            }
        }

        public void CreatePort(Direction direction, Port.Capacity capacity, string portName, int id)
        {
            var port = new NodePort(this, Orientation.Horizontal, direction, capacity, typeof(Port), id);
            port.portName = portName;

            switch (direction)
            {
                case Direction.Input:
                    m_InputPorts.Add(port);
                    inputContainer.Add(port);
                    break;
                case Direction.Output:
                    m_OutputPort = port;
                    outputContainer.Add(port);
                    break;
            }
        }

        protected virtual void Draw()
        {
            title = m_NodeName;
        }
    }
}
