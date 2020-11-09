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
    [SerializeField] private LayerMask collisionUpLayer = new LayerMask();
    [SerializeField] private LayerMask dangerLayer = new LayerMask();
    public PlayManager playManager;
    public GameObject dustAnim;

    private SpriteRenderer m_Sprite;

    private float jumpStartHeight;
    private bool canJump;
    private bool canExtendJump;
    private bool stillHoldingJump;
    private bool transitionJump;
    private bool transitionJumpBuffer;
    private bool facingRight;
    private bool teleporting;

    private float speedStretch;
    private bool collisionDown;
    private bool collisionDownEnter;
    private float collisionDownTimer;

    private bool collisionUp;
    private bool collisionUpEnter;
    private bool collisionUpLeft;
    private bool collisionUpRight;
    private bool collisionUpMiddle;
    private bool collisionDanger;

    private float collisionLeftStep;
    private float collisionRightStep;
    private float detectDistance;

    private Rigidbody2D m_Rigidbody;
    
    private BoxCollider2D m_boxCollider;
    private Vector3 m_Velocity = Vector3.zero;

    private bool isInit = false;

    void Start()
    {
        if (!isInit) Init();
    }

    private void Init()
    {
        isInit = true;
        m_Sprite = GetComponent<SpriteRenderer>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        collisionDown = false;
        collisionUp = false;
        canJump = false;
        canExtendJump = false;
        transitionJumpBuffer = false;
        detectDistance = 0f;
        facingRight = true;
    }

    void FixedUpdate()
    {
        speedStretch = m_Rigidbody.velocity.y * Time.fixedDeltaTime;
        if (speedStretch > -0.05f) speedStretch = -0.05f;
        collisionDownEnter = collisionDown;
        collisionDown = CollisionUtilities.GetCollision(this.gameObject,
            //Vector3.down * (m_boxCollider.size.y / 2f), new Vector2(0.55f, -speedStretch * 2), layer, true);
            Vector3.down * (m_boxCollider.size.y / 2f), new Vector2(0.55f, 0.1f), layer, true);
        collisionDownEnter = collisionDown != collisionDownEnter && collisionDown;

        if (collisionDown) collisionDownTimer = coyoteTime;
        collisionDownTimer -= Time.fixedDeltaTime;

        //speedStretch = m_Rigidbody.velocity.y * Time.fixedDeltaTime;
        //if (speedStretch < 0.05f) speedStretch = 0.05f;
        collisionUpEnter = collisionUp;
        collisionUp = CollisionUtilities.GetCollision(this.gameObject,
            //Vector3.up * (m_boxCollider.size.y / 2f), new Vector2(0.5f, speedStretch * 2), collisionUpLayer, true);
            Vector3.up * (m_boxCollider.size.y / 2f), new Vector2(0.5f, 0.1f), collisionUpLayer, true);
        collisionUpEnter = collisionUp != collisionUpEnter;

        collisionUpLeft = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * (m_boxCollider.size.y / 2f + 0.1f) + Vector3.left * 0.24f, new Vector2(0.1f, 0.3f), collisionUpLayer);
        collisionUpRight = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * (m_boxCollider.size.y / 2f + 0.1f) + Vector3.right * 0.24f, new Vector2(0.1f, 0.3f), collisionUpLayer);
        collisionUpMiddle = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * (m_boxCollider.size.y / 2f + 0.1f), new Vector2(0.4f, 0.3f), collisionUpLayer);

        collisionDanger = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.zero, new Vector2(0.6f, 1.2f), dangerLayer);

        collisionLeftStep = CollisionUtilities.GetCollisionDistance(this.gameObject,
            Vector2.left * (m_boxCollider.size.x / 2f + (0.1f * detectDistance)), Vector2.down, m_boxCollider.size.y / 2f, layer);
        collisionRightStep = CollisionUtilities.GetCollisionDistance(this.gameObject,
            Vector2.right * (m_boxCollider.size.x  / 2f + (0.1f * detectDistance)), Vector2.down, m_boxCollider.size.y / 2f, layer);
    }

    public void Spawn(Vector3 spawnLocation, Vector3 spawnDirection)
    {
        if (!isInit) Init();
        m_Rigidbody.position = spawnLocation;
        if (spawnDirection.Equals(Vector3.down))
        {
            transitionJump = true;
        }
        else if (spawnDirection.Equals(Vector3.up))
        {
            m_Rigidbody.velocity = Vector3.zero;
        }
    }

    public void Move(float move, float speed, bool jump, bool extendJump)
    {
        Vector3 targetVelocity = new Vector2(move * speed, m_Rigidbody.velocity.y);
        m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, targetVelocity, ref m_Velocity, movementSmoothing * Time.fixedDeltaTime);

        if (collisionDanger)
        {
            playManager.Play(Vector3.left);
        }
        if (move > 0)
        {
            facingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (move < 0)
        {
            facingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (transitionJumpBuffer != transitionJump && transitionJump)
        {
            Debug.Log("Transition Jump!");
            jumpStartHeight = m_Rigidbody.position.y;
        }
        if (transitionJump)
        {
            canExtendJump = true;
            canJump = false;
            stillHoldingJump = true;
        }
        if (collisionDownTimer > 0f && jump && canJump)
        {
            jumpStartHeight = m_Rigidbody.position.y;
            canExtendJump = true;
            canJump = false;
            stillHoldingJump = true;
        }
        
        if (!jump) 
        {
            stillHoldingJump = false;
        }

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
        else if (collisionUpEnter && !transitionJump && (m_Rigidbody.velocity.y > 0 || !collisionDown))
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

        if ((extendJump || transitionJump) && canExtendJump && m_Rigidbody.position.y - jumpStartHeight < jumpHeight)
        {
            m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, jumpUpSpeed);
        }
        else
        {
            //Debug.Log("Stopped jump: " + extendJump + " " + transitionJump + " " + canExtendJump + " " + (m_Rigidbody.position.y - jumpStartHeight < jumpHeight));
            canExtendJump = false;
            transitionJump = false;
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
        transitionJumpBuffer = transitionJump;

        /* ANIMATIONS */

        if (collisionDownEnter) 
        {
            GameObject dustRight = Instantiate(dustAnim);
            dustRight.transform.position = transform.position + new Vector3(0.92f, -0.274f, 0f);
            GameObject dustLeft = Instantiate(dustAnim);
            dustLeft.transform.position += transform.position + new Vector3(-0.92f, -0.274f, 0f);
            dustLeft.transform.localScale = new Vector3(-1f, 1f, 1f);
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

    public LayerMask GetLayer()
    {
        return layer;
    }

    public LayerMask GetCollisionUpLayer()
    {
        return collisionUpLayer;
    }
}
