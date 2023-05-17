using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class StateMachinePoseNodeConfig : PoseNodeConfig
    {
        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            StateMachineNode stateMachineNode = new StateMachineNode();
            stateMachineNode.m_NodeConfig = this;
            stateMachineNode.InitializeGraphNode(graphRuntime);
            return stateMachineNode;
        }
    }
}
