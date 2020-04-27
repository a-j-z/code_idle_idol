using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController controller;

    public float speed = 40f;

    private float horizontalMove = 0f;
    private bool jump = false;

    public float jumpBuffer = 0.2f;
    private float m_jumpTimer;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        m_jumpTimer = 0f;
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * speed;
        if (Input.GetButtonDown("Jump"))
        {
            m_jumpTimer = jumpBuffer;
        }
        m_jumpTimer -= Time.deltaTime;
        jump = m_jumpTimer > 0;
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
    }
}
