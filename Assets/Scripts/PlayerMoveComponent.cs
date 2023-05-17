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
        var verticalInput = Input.GetAxis("Vertical");
        m_AnimationGraphPlayer.SetFloatParameter("moveSpeed", verticalInput);
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_AnimationGraphPlayer.SetBoolParameter("mode", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_AnimationGraphPlayer.SetBoolParameter("mode", false);
        }

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
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_AnimationGraphPlayer.SetBoolParameter("moveCondition", true);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_AnimationGraphPlayer.SetBoolParameter("moveCondition", false);
        }
    }
}
