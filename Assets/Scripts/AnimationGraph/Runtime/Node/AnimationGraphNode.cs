using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimationGraph
{
    public abstract class AnimationGraphNode<TNodeConfig> where TNodeConfig : NodeConfig
    {
        private TNodeConfig m_NodeConfig;
    }
}
