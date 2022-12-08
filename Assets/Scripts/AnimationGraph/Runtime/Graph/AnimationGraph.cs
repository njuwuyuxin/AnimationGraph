using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    [CreateAssetMenu(fileName = "AnimationGraph", menuName = "ScriptableObjects/AnimationGraph")]
    public class AnimationGraph : ScriptableObject
    {
        public FinalPosePoseNodeConfig finalPosePoseNode;

        [SerializeReference]
        [SerializeReferenceButton]
        public List<NodeConfig> nodes;

        public List<Connection> nodeConnections;

        [SerializeReference]
        [SerializeReferenceButton]
        public List<GraphParameter> parameters;
    }

    [Serializable]
    public class Connection
    {
        public int sourceNodeId;
        public int targetNodeId;
        public int targetSlotIndex;
    }
}
