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
            m_AnimationGraphPlayer.SetStringParameter("stringParameter", "Run");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            m_AnimationGraphPlayer.SetStringParameter("stringParameter", "Idle");
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            m_AnimationGraphPlayer.SetStringParameter("stringParameter", "LeftStrafe");
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            m_AnimationGraphPlayer.SetStringParameter("stringParameter", "RightStrafe");
        }
    }
}
