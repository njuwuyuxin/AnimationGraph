
using System;
using UnityEditor;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public abstract class NodeConfig
    {
        public int id;

        public NodeConfig()
        {
            if (id == 0)
            {
                id = Animator.StringToHash(GUID.Generate().ToString());
            }
        }

        public abstract IAnimationGraphNodeInterface GenerateAnimationGraphNode(AnimationGraphRuntime graphRuntime);
    }
}
