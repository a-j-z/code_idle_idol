using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float movementSmoothing = 0.1f;
    public float gravity = 10f;
    public float gravityMultiplier = 2f;
    public float jumpForce = 400f;
    [SerializeField] private LayerMask layer = new LayerMask();
    
    private bool collisionDown;
    private bool collisionDownEnter;

    private Rigidbody2D m_Rigidbody;
    private BoxCollider2D m_boxCollider;
    private Vector3 m_Velocity = Vector3.zero;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        collisionDown = false;
    }
    void FixedUpdate()
    {
        collisionDownEnter = collisionDown;
        collisionDown = GetCollision(Vector3.down * (m_boxCollider.size.y / 2f),
            new Vector2(m_boxCollider.size.x * 0.9f, m_boxCollider.size.x * 0.5f), layer);
        
        collisionDownEnter = collisionDown != collisionDownEnter;
    }

    public void Move(float move, bool jump)
    {
        Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody.velocity.y);
        m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, targetVelocity, ref m_Velocity, movementSmoothing);

        if (collisionDown && jump)
        {
            m_Rigidbody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        if (jump && m_Rigidbody.velocity.y >= 0)
        {
            m_Rigidbody.gravityScale = gravity * gravityMultiplier;
        }
        else
        {
            m_Rigidbody.gravityScale = gravity;
        }
    }

    private bool GetCollision(Vector3 positionOffset, Vector2 overlapBoxDims, LayerMask layer) //Add layer input
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + positionOffset, overlapBoxDims, 0);

        Debug.DrawLine(
            transform.position + positionOffset + new Vector3(-overlapBoxDims.x * 0.5f, overlapBoxDims.y * 0.5f, 0),
            transform.position + positionOffset + new Vector3(overlapBoxDims.x * 0.5f, overlapBoxDims.y * 0.5f, 0),
            Color.red);
        Debug.DrawLine(
            transform.position + positionOffset + new Vector3(-overlapBoxDims.x * 0.5f, -overlapBoxDims.y * 0.5f, 0),
            transform.position + positionOffset + new Vector3(overlapBoxDims.x * 0.5f, -overlapBoxDims.y * 0.5f, 0),
            Color.red);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }
}
