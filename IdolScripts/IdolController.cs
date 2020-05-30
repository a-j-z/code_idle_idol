using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdolController : MonoBehaviour
{
    public float pickupSmooth = 3f;
    public float throwPeakSmooth = 70f;
    public float carryHeight = 1.2f;
    public float carryWidth = 0.6f;
    public float throwHeight = 3f;
    public float throwPower = 2f;
    public float riseSpeed = 10f;
    public float fallSpeed = 15f;
    public float dropRadius = 1.5f;
    public float teleportDuration = 0.5f;
    public float teleportRechargeDuration = 0.5f;
    public PlayerController player;
    public TeleportRecharge rechargeIndicator;
    [SerializeField] private LayerMask layer = new LayerMask();
    [SerializeField] private LayerMask collisionUpLayer = new LayerMask();
    [SerializeField] private LayerMask idolLayer = new LayerMask();
    [SerializeField] private LayerMask carriedIdolLayer = new LayerMask();
    private LayerMask collisionDownLayer;

    private Rigidbody2D m_Rigidbody;
    private Rigidbody2D playerRigidbody;
    private BoxCollider2D m_boxCollider;

    private float speedStretch;
    private bool collisionDown;
    private bool collisionDownEnter;
    private bool collisionUp;
    private bool collisionUpEnter;
    private bool collisionLeft;
    private bool collisionRight;
    private float collisionCarryPlayer;
    private float collisionCarry;
    private bool collisionTeleport;
    private bool canExtendThrow;

    private bool canCarry;
    private bool isCarried;
    private bool isThrown;
    private float throwStartHeight;
    private Vector2 m_Velocity = Vector2.zero;

    private bool canTeleport;
    private float teleportTimer;
    private bool teleportBuffer;
    private float teleportRechargeTimer;
    private Vector2 velocityBuffer;
    private Vector2 playerVelocityBuffer;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        playerRigidbody = player.GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        canCarry = true;
        isCarried = false;
        isThrown = false;
        canExtendThrow = false;
        canTeleport = true;
        collisionCarry = 0f;
        teleportTimer = 0f;
        teleportBuffer = false;
    }

    void FixedUpdate()
    {
        if (isCarried)
        {
            collisionDownLayer = collisionUpLayer;
            gameObject.layer = LayerUtilities.LayerNumber(carriedIdolLayer);
        }
        else
        {
            collisionDownLayer = layer;
            gameObject.layer = LayerUtilities.LayerNumber(idolLayer);
        }

        speedStretch = m_Rigidbody.velocity.y * Time.fixedDeltaTime;
        if (speedStretch > -0.05f) speedStretch = -0.05f;
        collisionDownEnter = collisionDown;
        collisionDown = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.down * (m_boxCollider.size.y / 2f), 
            new Vector2(0.25f, (-speedStretch + 0.05f) * 2f), collisionDownLayer, true);
        collisionDownEnter = collisionDown != collisionDownEnter;

        speedStretch = m_Rigidbody.velocity.y * Time.fixedDeltaTime;
        if (speedStretch < 0.05f) speedStretch = 0.05f;
        collisionUpEnter = collisionUp;
        collisionUp = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * (m_boxCollider.size.y / 2f + 0.05f), new Vector2(0.25f, (speedStretch + 0.05f) * 2f), collisionUpLayer);
        collisionUpEnter = collisionUp != collisionUpEnter;

        collisionLeft = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.left * (m_boxCollider.size.x / 2f) + Vector3.down * 0.05f, new Vector2(0.1f, 0.5f), collisionUpLayer);

        collisionRight = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.right * (m_boxCollider.size.x / 2f) + Vector3.down * 0.05f, new Vector2(0.1f, 0.5f), collisionUpLayer);

        collisionCarryPlayer = CollisionUtilities.GetCollisionDistance(player.gameObject,
            Vector2.zero, Vector2.up, carryHeight + 0.6f, player.GetCollisionUpLayer());

        collisionCarry = CollisionUtilities.GetCollisionDistance(player.gameObject,
            new Vector2(m_Rigidbody.position.x - playerRigidbody.position.x, 0f),
            Vector2.up, carryHeight + 0.6f, collisionUpLayer);

        collisionTeleport = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * 0.35f + Vector3.left * 0.225f, new Vector2(0.15f, 1.1f), collisionUpLayer) &&
            CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * 0.35f + Vector3.right * 0.225f, new Vector2(0.15f, 1.1f), collisionUpLayer);

        float n; if (teleportRechargeTimer < 0) n = 0; else n = teleportRechargeTimer; /////
        CollisionUtilities.GetCollision(this.gameObject,
            Vector3.right * 0.5f, new Vector2(0.1f, n * 3f), layer, true);

        teleportTimer -= Time.fixedDeltaTime;
        teleportRechargeTimer -= Time.fixedDeltaTime;
        rechargeIndicator.SetFill(teleportRechargeTimer / teleportRechargeDuration);
    }

    public void Move(bool interact, bool extendInteract, bool interactSecondary, bool teleport)
    {
        //if (collisionDownEnter && m_Rigidbody.velocity.y <= 0)
        //{
        //    m_Rigidbody.velocity = new Vector2(0, m_Rigidbody.velocity.y);
        //}
        Debug.Log(m_Rigidbody.velocity.y);
        if (collisionDown && m_Rigidbody.velocity.y <= 0.1f && !isCarried)
        {
            //m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, 0f);
            m_Rigidbody.velocity = new Vector2(0, m_Rigidbody.velocity.y);
            m_Rigidbody.velocity -= new Vector2(0, throwPeakSmooth * Time.fixedDeltaTime);
            if (m_Rigidbody.velocity.y < -fallSpeed)
            {
                m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, -fallSpeed);
            }
        }
        else if (isThrown && m_Rigidbody.position.y - throwStartHeight < throwHeight && canExtendThrow)
        {
            m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, riseSpeed);
        }
        else if (!isCarried)
        {
            isThrown = false;
            m_Rigidbody.velocity -= new Vector2(0, throwPeakSmooth * Time.fixedDeltaTime);
            if (m_Rigidbody.velocity.y < -fallSpeed)
            {
                m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, -fallSpeed);
            }
        }

        if (teleport && canTeleport && !isCarried && teleportRechargeTimer < 0)
        {
            if (!collisionTeleport)
            {
                velocityBuffer = m_Rigidbody.velocity;
                playerVelocityBuffer = playerRigidbody.velocity;
                teleportTimer = teleportDuration;
                teleportBuffer = true;
                canTeleport = false;
                player.SetTeleporting(true);
            }
            else
            {
                //SHOW PLAYER THAT THEY CAN'T TELEPORT HERE
            }
        }
        if (teleportTimer >= 0 && teleportTimer != teleportDuration)
        {
            playerRigidbody.position = m_Rigidbody.position + Vector2.up * 0.3f;
            m_Rigidbody.velocity = Vector2.zero;
        }
        if (teleportBuffer && teleportTimer < 0)
        {
            player.SetTeleporting(false);
            playerRigidbody.velocity = playerVelocityBuffer;
            m_Rigidbody.velocity = velocityBuffer;
            teleportBuffer = false;
            teleportRechargeTimer = teleportRechargeDuration;
            Carry(true);
        }
        if (!teleport)
        {
            canTeleport = true;
        }
        
        if (collisionUp && canExtendThrow && !isCarried)
        {
            canExtendThrow = false;
            m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, 0f);
        }

        if (interact && canCarry && teleportTimer < 0 &&
            ((Mathf.Abs(playerRigidbody.position.x - m_Rigidbody.position.x) <= 1f &&
            Mathf.Abs(playerRigidbody.position.y - m_Rigidbody.position.y) <= 1.5f) ||
            isCarried))
        {
            Carry();
        }
        if (interactSecondary && isCarried)
        {
            Drop();
        }
        if (!extendInteract && !interact && !interactSecondary)
        {
            canCarry = true;
        }

        if (isCarried)
        {
            m_Rigidbody.velocity = Vector2.zero;
            bool canMoveX = true;
            bool canMoveY = true;
            if (playerRigidbody.position.x + carryWidth > m_Rigidbody.position.x && collisionRight)
            {
                canMoveX = false;
                if (playerRigidbody.position.x - m_Rigidbody.position.x > dropRadius) Drop();
            }
            else if (playerRigidbody.position.x - carryWidth < m_Rigidbody.position.x && collisionLeft)
            {
                canMoveX = false;
                if (playerRigidbody.position.x - m_Rigidbody.position.x < -dropRadius) Drop();
            }
            if (playerRigidbody.position.y + collisionCarryPlayer - 0.6f > m_Rigidbody.position.y && collisionUp)
            {
                canMoveY = false;
                if (playerRigidbody.position.y - m_Rigidbody.position.y > dropRadius) Drop();
            }
            else if (playerRigidbody.position.y + collisionCarryPlayer - 0.6f < m_Rigidbody.position.y && collisionDown)
            {
                canMoveY = false;
                if (playerRigidbody.position.y - m_Rigidbody.position.y < -dropRadius) Drop();
            }
            Vector2 destination;
            int direction = player.GetFacingRight() ? -1 : 1;
            float jumping = 1.0f - ((collisionCarryPlayer - 0.6f) / 1.2f);
            float height = Mathf.Min(collisionCarryPlayer, collisionCarry);

            destination = new Vector2(
                canMoveX ? playerRigidbody.position.x + (carryWidth * direction * jumping): m_Rigidbody.position.x,
                canMoveY ? playerRigidbody.position.y + height - 0.6f: m_Rigidbody.position.y);
            m_Rigidbody.position = Vector2.SmoothDamp(m_Rigidbody.position, destination, ref m_Velocity, pickupSmooth * Time.fixedDeltaTime);
        }
    }

    private void Carry(bool teleporting = false)
    {
        if (isCarried)
        {
            m_Rigidbody.velocity = new Vector2(playerRigidbody.velocity.x * 2f, m_Rigidbody.velocity.y);
            throwStartHeight = playerRigidbody.position.y + carryHeight;
            isThrown = true;
            canExtendThrow = true;
        }
        isCarried = !isCarried;
        canCarry = false;
    }

    private void Drop()
    {
        m_Rigidbody.velocity = new Vector2(playerRigidbody.velocity.x, m_Rigidbody.velocity.y);
        isCarried = false;
        canCarry = false;
    }
}
