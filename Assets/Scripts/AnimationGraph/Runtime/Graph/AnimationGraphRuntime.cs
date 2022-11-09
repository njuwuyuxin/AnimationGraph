using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{

    public class AnimationGraphRuntime
    {
        private AnimationActor m_Actor;
        private AnimationGraph m_AnimationGraph;
        public PlayableGraph playableGraph => m_PlayableGraph;
        private PlayableGraph m_PlayableGraph;
        private AnimationPlayableOutput m_Output;
        private FinalPoseNode m_FinalPoseNode;
        private Dictionary<int, IAnimationGraphNodeInterface> m_Id2NodeMap;
        
        public AnimationGraphRuntime(AnimationActor actor, AnimationGraph animationGraph)
        {
            m_Actor = actor;
            m_AnimationGraph = animationGraph;
            m_PlayableGraph = PlayableGraph.Create();
            m_PlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            m_Output = AnimationPlayableOutput.Create(m_PlayableGraph,
                m_Actor.gameObject.name + "_" + m_Actor.gameObject.GetInstanceID(), m_Actor.animator);
            GenerateAnimationGraph();
        }

        public void Run()
        {
            var finalPlayable = m_FinalPoseNode.GetPlayable();
            m_Output.SetSourcePlayable(finalPlayable);
            
            m_PlayableGraph.Play();
        }

        private void GenerateAnimationGraph()
        {
            m_Id2NodeMap = new Dictionary<int, IAnimationGraphNodeInterface>();
            m_FinalPoseNode = (FinalPoseNode)m_AnimationGraph.finalPoseNode.GenerateAnimationGraphNode(this);
            m_Id2NodeMap.Add(m_AnimationGraph.finalPoseNode.id, m_FinalPoseNode);
            foreach (var nodeConfig in m_AnimationGraph.nodes)
            {
                var animationGraphNode = nodeConfig.GenerateAnimationGraphNode(this);
                m_Id2NodeMap.Add(nodeConfig.id, animationGraphNode);
            }

            foreach (var connection in m_AnimationGraph.nodeConnections)
            {
                var sourceNode = m_Id2NodeMap[connection.sourceNodeId];
                var targetNode = m_Id2NodeMap[connection.targetNodeId];
                sourceNode.AddOutputNodes(targetNode);
                targetNode.AddInputNodes(sourceNode);
            }
        }

        public void OnUpdate(float deltaTime)
        {
            
        }

        public void Destroy()
        {
            m_PlayableGraph.Destroy();
        }
    }
}
