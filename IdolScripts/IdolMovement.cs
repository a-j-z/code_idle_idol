using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdolMovement : MonoBehaviour
{
    public float interactBuffer;

    private IdolController controller;
    private float m_InteractTimer;
    private bool interact;
    private bool interactSecondary;
    private bool extendInteract;


    void Start()
    {
        controller = GetComponent<IdolController>();
        interact = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            m_InteractTimer = interactBuffer;
        }
        m_InteractTimer -= Time.deltaTime;
        interact = m_InteractTimer > 0;
        extendInteract = Input.GetButton("Fire1");
        interactSecondary = Input.GetButton("Fire2");
    }    

    void FixedUpdate()
    {
        controller.Move(interact, extendInteract, interactSecondary);
    }
}
