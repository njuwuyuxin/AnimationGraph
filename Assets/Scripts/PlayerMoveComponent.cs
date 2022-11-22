using System.Collections;
using System.Collections.Generic;
using AnimationGraph;
using UnityEngine;

public class PlayerMoveComponent : MonoBehaviour
{
    private AnimationGraphPlayer m_AnimationGraphPlayer;

    void Start()
    {
        m_AnimationGraphPlayer = GetComponent<AnimationGraphPlayer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_AnimationGraphPlayer.SetBoolParameter("testBool", true);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            m_AnimationGraphPlayer.SetBoolParameter("testBool", false);
        }
    }
}
