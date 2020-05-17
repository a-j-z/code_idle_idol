using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController controller;

    public float speed = 40f;

    private float horizontalMove = 0f;
    private bool jump = false;
    private bool extendJump = false;

    public float jumpBuffer = 0.2f;
    private float m_jumpTimer;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        m_jumpTimer = 0f;
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            m_jumpTimer = jumpBuffer;
        }
        m_jumpTimer -= Time.deltaTime;
        jump = m_jumpTimer > 0;
        extendJump = Input.GetButton("Jump");

    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove, speed  * Time.fixedDeltaTime, jump, extendJump);
    }
}
