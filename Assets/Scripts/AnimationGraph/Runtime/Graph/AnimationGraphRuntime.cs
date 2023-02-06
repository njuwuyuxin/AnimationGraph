using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class AnimationGraphRuntime
    {
        private AnimationActor m_Actor;
        private AnimationGraph m_AnimationGraph;
        public PlayableGraph m_PlayableGraph;
        private AnimationPlayableOutput m_Output;
        public Playable m_FinalPlayable;
        private FinalPoseNode m_FinalPoseNode;
        private Dictionary<int, INode> m_Id2NodeMap;
        private Dictionary<int, GraphParameter> m_Id2ParameterMap;
        
        public AnimationGraphRuntime(AnimationActor actor, AnimationGraph animationGraph)
        {
            m_Actor = actor;
            m_AnimationGraph = animationGraph;
            m_PlayableGraph = PlayableGraph.Create();
            m_PlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            m_Output = AnimationPlayableOutput.Create(m_PlayableGraph,
                m_Actor.gameObject.name + "_" + m_Actor.gameObject.GetInstanceID(), m_Actor.animator);
            GenerateAnimationGraph();
            m_FinalPlayable = Playable.Create(m_PlayableGraph, 1);
            m_Output.SetSourcePlayable(m_FinalPlayable);
        }

        public void Run()
        {
            m_FinalPoseNode.OnStart();
            m_PlayableGraph.Play();
        }

        private void GenerateAnimationGraph()
        {
            GenerateParameterMap();
            
            m_Id2NodeMap = new Dictionary<int, INode>();
            m_FinalPoseNode = (FinalPoseNode)m_AnimationGraph.finalPosePoseNode.GenerateNode(this);
            m_Id2NodeMap.Add(m_AnimationGraph.finalPosePoseNode.id, m_FinalPoseNode);
            foreach (var nodeConfig in m_AnimationGraph.nodes)
            {
                if (m_Id2NodeMap.ContainsKey(nodeConfig.id))
                {
                    continue;
                }
                var animationGraphNode = nodeConfig.GenerateNode(this);
                m_Id2NodeMap.Add(nodeConfig.id, animationGraphNode);
            }

            foreach (var connection in m_AnimationGraph.nodeConnections)
            {
                var sourceNode = m_Id2NodeMap[connection.sourceNodeId];
                var targetNode = m_Id2NodeMap[connection.targetNodeId];
                sourceNode.AddOutputNode(targetNode);
                targetNode.AddInputNode(sourceNode, connection.targetSlotIndex);
            }
        }

        private void GenerateParameterMap()
        {
            m_Id2ParameterMap = new Dictionary<int, GraphParameter>();
            foreach (var parameter in m_AnimationGraph.parameters)
            {
                m_Id2ParameterMap.Add(Animator.StringToHash(parameter.name), parameter);
            }
        }

        public void OnUpdate(float deltaTime)
        {
            m_FinalPoseNode.OnUpdate(deltaTime);
        }

        public void Destroy()
        {
            m_PlayableGraph.Destroy();
        }

        public void SetBoolParameter(string parameterName, bool value)
        {
            var hash = Animator.StringToHash(parameterName);
            if (m_Id2ParameterMap.TryGetValue(hash, out var graphParameter))
            {
                foreach (var nodeId in  graphParameter.associatedNodes)
                {
                    if (m_Id2NodeMap.TryGetValue(nodeId, out var node))
                    {
                        BoolValueNode boolValueNode = (BoolValueNode) node;
                        boolValueNode.boolValue = value;
                    }
                }
            }
            else
            {
                Debug.LogError("Paramter \"" + parameterName + "\" not exist!");
            }
        }
    }
}
