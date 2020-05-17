using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdolMovement : MonoBehaviour
{
    public PlayerInputManager inputManager;
    public float interactBuffer;
    public float teleportBuffer;

    private IdolController controller;

    private float m_InteractTimer;
    private bool interact;

    private float m_TeleportTimer;
    private bool teleport;

    private bool interactSecondary;
    private bool extendInteract;


    void Start()
    {
        controller = GetComponent<IdolController>();
        interact = false;
    }

    void Update()
    {
        if (inputManager.GetInput("interact", "down"))
        {
            m_InteractTimer = interactBuffer;
        }
        m_InteractTimer -= Time.deltaTime;
        interact = m_InteractTimer > 0;

        if (inputManager.GetInput("teleport", "down"))
        {
            m_TeleportTimer = teleportBuffer;
        }
        m_TeleportTimer -= Time.deltaTime;
        teleport = m_TeleportTimer > 0;

        extendInteract = inputManager.GetInput("interact");
        interactSecondary = inputManager.GetInput("interactSecondary");
    }    

    void FixedUpdate()
    {
        controller.Move(interact, extendInteract, interactSecondary, teleport);
    }
}
