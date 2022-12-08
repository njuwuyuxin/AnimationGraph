using System;
using UnityEditor;
using UnityEngine;

namespace AnimationGraph
{
    public enum ENodeType
    {
        PoseNode,
        ValueNode,
    }
    
    [Serializable]
    public abstract class NodeConfig
    {
        [SerializeField]
        public int id;
        public virtual ENodeType nodeType { get; }

        public NodeConfig()
        {
            if (id == 0)
            {
                id = Animator.StringToHash(  Guid.NewGuid().ToString());
            }
        }

        public abstract INode GenerateNode(AnimationGraphRuntime graphRuntime);
    }
}
