using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class AnimationClipNodeConfig : NodeConfig
    {
        public AnimationClip clip;
        public float playSpeed;
    }
}
