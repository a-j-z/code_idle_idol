using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSmoothing = 3f;
    public float jumpUpSpeed = 10f;
    public float jumpDownSpeed = 10f;
    public float jumpHeight = 3f;
    public float jumpPeakSmooth = 200f;
    public float coyoteTime = 0.1f;
    [SerializeField] private LayerMask layer = new LayerMask();

    private SpriteRenderer m_Sprite;

    private float jumpStartHeight;
    private bool canJump;
    private bool canExtendJump;
    private bool stillHoldingJump;
    private bool facingRight;
    private bool teleporting;

    private bool collisionDown;
    private bool collisionDownEnter;
    private float collisionDownTimer;

    private bool collisionUp;
    private bool collisionUpEnter;
    private bool collisionUpLeft;
    private bool collisionUpRight;
    private bool collisionUpMiddle;

    private float collisionLeftStep;
    private float collisionRightStep;
    private float detectDistance;

    private Rigidbody2D m_Rigidbody;
    private BoxCollider2D m_boxCollider;
    private Vector3 m_Velocity = Vector3.zero;

    void Start()
    {
        m_Sprite = GetComponent<SpriteRenderer>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        collisionDown = false;
        collisionUp = false;
        canJump = false;
        canExtendJump = false;
        detectDistance = 0f;
        facingRight = true;
    }
    void FixedUpdate()
    {
        collisionDownEnter = collisionDown;
        collisionDown = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.down * (m_boxCollider.size.y / 2f), new Vector2(0.55f, 0.1f), layer);
        collisionDownEnter = collisionDown != collisionDownEnter;

        if (collisionDown) collisionDownTimer = coyoteTime;
        collisionDownTimer -= Time.fixedDeltaTime;

        collisionUpEnter = collisionUp;
        collisionUp = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * (m_boxCollider.size.y / 2f), new Vector2(0.5f, 0.1f), layer);
        collisionUpEnter = collisionUp != collisionUpEnter;

        collisionUpLeft = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * (m_boxCollider.size.y / 2f + 0.1f) + Vector3.left * 0.24f, new Vector2(0.1f, 0.3f), layer);
        collisionUpRight = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * (m_boxCollider.size.y / 2f + 0.1f) + Vector3.right * 0.24f, new Vector2(0.1f, 0.3f), layer);
        collisionUpMiddle = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * (m_boxCollider.size.y / 2f + 0.1f), new Vector2(0.4f, 0.3f), layer);

        collisionLeftStep = CollisionUtilities.GetCollisionDistance(this.gameObject,
            Vector2.left * (m_boxCollider.size.x / 2f + (0.1f * detectDistance)), Vector2.down, m_boxCollider.size.y / 2f, layer);
        collisionRightStep = CollisionUtilities.GetCollisionDistance(this.gameObject,
            Vector2.right * (m_boxCollider.size.x  / 2f + (0.1f * detectDistance)), Vector2.down, m_boxCollider.size.y / 2f, layer);
    }

    public void Move(float move, float speed, bool jump, bool extendJump)
    {
        Vector3 targetVelocity = new Vector2(move * speed, m_Rigidbody.velocity.y);
        m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, targetVelocity, ref m_Velocity, movementSmoothing * Time.fixedDeltaTime);

        if (move > 0)
        {
            facingRight = true;
        }
        else if (move < 0)
        {
            facingRight = false;
        }

        if (collisionDownTimer > 0f && jump && canJump)
        {
            jumpStartHeight = transform.position.y;
            canExtendJump = true;
            canJump = false;
            stillHoldingJump = true;
        }
        if (!jump) stillHoldingJump = false;
        if (collisionDown && !stillHoldingJump)
        {
            canJump = true;
        }

        if (collisionUpLeft && !collisionUpMiddle && !collisionDown && m_Rigidbody.velocity.y > 0)
        {
            m_Rigidbody.position += Vector2.right * 0.1f;
        }
        else if (collisionUpRight && !collisionUpMiddle && !collisionDown && m_Rigidbody.velocity.y > 0)
        {
            m_Rigidbody.position += Vector2.left * 0.1f;
        }
        else if (collisionUpEnter)
        {
            canExtendJump = false;
            canJump = false;
            m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, 0);
        }

        detectDistance = Mathf.Abs(m_Rigidbody.velocity.x) / speed;
        if (collisionLeftStep > 0.1f && collisionLeftStep < 0.6f && m_Rigidbody.velocity.x < 0 && collisionDown)
        {
            m_Rigidbody.position += Vector2.up * ((0.6f - collisionLeftStep) + 0.1f);
        }
        else if (collisionRightStep > 0.1f && collisionRightStep < 0.6f && m_Rigidbody.velocity.x > 0 && collisionDown)
        {
            m_Rigidbody.position += Vector2.up * ((0.6f - collisionRightStep) + 0.1f);
        }

        if (extendJump && canExtendJump && transform.position.y - jumpStartHeight < jumpHeight)
        {
            m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, jumpUpSpeed);
        }
        else
        {
            canExtendJump = false;
            m_Rigidbody.velocity -= new Vector2(0, jumpPeakSmooth * Time.fixedDeltaTime);
            if (m_Rigidbody.velocity.y < -jumpDownSpeed)
            {
                m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, -jumpDownSpeed);
            }
        }

        if (teleporting)
        {
            m_Sprite.enabled = false;
            m_Rigidbody.velocity = Vector2.zero;
        }
        else
        {
            m_Sprite.enabled = true;
        }
    }

    public bool GetFacingRight()
    {
        return facingRight;
    }

    public void SetTeleporting(bool teleporting)
    {
        this.teleporting = teleporting;
    }
}
