using System;
using System.Collections.Generic;

namespace AnimationGraph
{
    [Serializable]
    public class Transition
    {
        public int sourceStateId;
        public int targetStateId;
    }
    
    [Serializable]
    public class StateMachinePoseNodeConfig : PoseNodeConfig
    {
        public List<StatePoseNodeConfig> states;
        public List<Transition> transitions;
        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            StateMachineNode stateMachineNode = new StateMachineNode();
            stateMachineNode.m_NodeConfig = this;
            stateMachineNode.InitializeGraphNode(graphRuntime);
            return stateMachineNode;
        }
    }
}
