using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class AnimationGraphPlayer : MonoBehaviour
    {
        public CompiledAnimationGraph animationGraph;

        private AnimationGraphRuntime m_AnimationGraphRuntime;
        private AnimationActor m_Actor;

        void Start()
        {
            m_Actor = new AnimationActor(gameObject);
            m_AnimationGraphRuntime = new AnimationGraphRuntime(m_Actor, animationGraph);
            m_AnimationGraphRuntime.Run();
        }

        void Update()
        {
            m_AnimationGraphRuntime.OnUpdate(Time.deltaTime);
        }

        private void OnDestroy()
        {
            m_AnimationGraphRuntime.Destroy();
        }
        
        public void SetBoolParameter(string parameterName, bool value)
        {
            m_AnimationGraphRuntime.SetBoolParameter(parameterName, value);
        }
    }
}
