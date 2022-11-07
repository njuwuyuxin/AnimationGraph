using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    [CreateAssetMenu(fileName = "AnimationGraph", menuName = "ScriptableObjects/AnimationGraph")]
    public class AnimationGraph : ScriptableObject
    {
        public AnimationClip clip;
    }
}
