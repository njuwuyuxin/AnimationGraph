using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimationGraph
{

    public abstract class INode
    {
        public virtual ENodeType nodeType { get; }

        public abstract void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime);
        
        protected void OnUpdate(float deltaTime){}

        public virtual void AddInputNode(INode inputNode, int slotIndex)
        {
            throw new NotImplementedException();
        }
        
        public virtual void AddOutputNode(INode outputNode)
        {
            throw new NotImplementedException();
        }
    }
}
