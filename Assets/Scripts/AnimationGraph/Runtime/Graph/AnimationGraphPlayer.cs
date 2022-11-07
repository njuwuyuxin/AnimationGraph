using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class AnimationGraphPlayer : MonoBehaviour
    {
        public AnimationGraph animationGraph;

        private Animator m_Animator;
        private PlayableGraph m_Graph;
        private AnimationPlayableOutput m_Output;

        void Start()
        {
            m_Animator = GetComponent<Animator>();
            m_Graph = PlayableGraph.Create();
            m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            m_Output = AnimationPlayableOutput.Create(m_Graph, "MainGraph", m_Animator);

            AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(m_Graph, animationGraph.clip);
            m_Output.SetSourcePlayable(clipPlayable);
            m_Graph.Play();
        }

        void Update()
        {

        }

        private void OnDestroy()
        {
            m_Graph.Destroy();
        }
    }
}
