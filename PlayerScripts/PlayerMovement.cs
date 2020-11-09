using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerInputManager inputManager;
    public float speed = 40f;

    private PlayerController controller;
    

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
        if (inputManager.GetInput("jump", "down"))
        {
            m_jumpTimer = jumpBuffer;
        }
        m_jumpTimer -= Time.deltaTime;
        jump = m_jumpTimer > 0;
        extendJump = inputManager.GetInput("jump");  
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove, speed  * Time.fixedDeltaTime, jump, extendJump);
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
