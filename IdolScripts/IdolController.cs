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
    public PlayerController player;
    [SerializeField] private LayerMask layer = new LayerMask();

    private Rigidbody2D m_Rigidbody;
    private BoxCollider2D m_boxCollider;

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

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        canCarry = true;
        isCarried = false;
        isThrown = false;
        canExtendThrow = false;
        canTeleport = true;
        collisionCarry = 0f;
    }

    void FixedUpdate()
    {
        collisionDownEnter = collisionDown;
        collisionDown = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.down * (m_boxCollider.size.y / 2f), new Vector2(0.25f, 0.1f), layer);
        collisionDownEnter = collisionDown != collisionDownEnter;

        collisionUpEnter = collisionUp;
        collisionUp = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * (m_boxCollider.size.y / 2f + 0.05f), new Vector2(0.25f, 0.2f), layer);
        collisionUpEnter = collisionUp != collisionUpEnter;

        collisionLeft = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.left * (m_boxCollider.size.x / 2f) + Vector3.down * 0.05f, new Vector2(0.1f, 0.5f), layer);

        collisionRight = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.right * (m_boxCollider.size.x / 2f) + Vector3.down * 0.05f, new Vector2(0.1f, 0.5f), layer);

        collisionCarryPlayer = CollisionUtilities.GetCollisionDistance(player.gameObject,
            Vector2.zero, Vector2.up, carryHeight + 0.6f, layer, true);

        collisionCarry = CollisionUtilities.GetCollisionDistance(player.gameObject,
            new Vector2(m_Rigidbody.position.x - player.GetComponent<Rigidbody2D>().position.x, 0f), Vector2.up, carryHeight + 0.6f, layer, true);

        collisionTeleport = CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * 0.35f + Vector3.left * 0.225f, new Vector2(0.15f, 1.1f), layer) &&
            CollisionUtilities.GetCollision(this.gameObject,
            Vector3.up * 0.35f + Vector3.right * 0.225f, new Vector2(0.15f, 1.1f), layer);
    }

    public void Move(bool interact, bool extendInteract, bool interactSecondary, bool teleport)
    {
        if (collisionDownEnter)
        {
            m_Rigidbody.velocity = Vector2.zero;
        }

        if (collisionDown)
        {
            m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, 0f);
        }
        else if (isThrown && m_Rigidbody.position.y - throwStartHeight < throwHeight && canExtendThrow)
        {
            m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, riseSpeed);
        }
        else
        {
            isThrown = false;
            m_Rigidbody.velocity -= new Vector2(0, throwPeakSmooth * Time.deltaTime);
            if (m_Rigidbody.velocity.y < -fallSpeed)
            {
                m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, -fallSpeed);
            }
        }

        if (teleport && canTeleport && !isCarried)
        {
            if (!collisionTeleport)
            {
                player.GetComponent<Rigidbody2D>().position = m_Rigidbody.position + Vector2.up * 0.3f;
                canTeleport = false;
                Carry(true);
            }
            else
            {
                //SHOW PLAYER THAT THEY CAN'T TELEPORT HERE
            }
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

        if (interact && canCarry &&
            ((Mathf.Abs(player.GetComponent<Rigidbody2D>().position.x - m_Rigidbody.position.x) <= 1f &&
            Mathf.Abs(player.GetComponent<Rigidbody2D>().position.y - m_Rigidbody.position.y) <= 1.5f) ||
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
            if (player.GetComponent<Rigidbody2D>().position.x + carryWidth > m_Rigidbody.position.x && collisionRight)
            {
                canMoveX = false;
                if (player.GetComponent<Rigidbody2D>().position.x - m_Rigidbody.position.x > 1f)
                {
                    Drop();
                }
            }
            else if (player.GetComponent<Rigidbody2D>().position.x - carryWidth < m_Rigidbody.position.x && collisionLeft)
            {
                canMoveX = false;
                if (player.GetComponent<Rigidbody2D>().position.x - m_Rigidbody.position.x < -1f)
                {
                    Drop();
                }
            }
            if (player.GetComponent<Rigidbody2D>().position.y + collisionCarryPlayer - 0.6f> m_Rigidbody.position.y && collisionUp)
            {
                canMoveY = false;
            }
            Vector2 destination;
            int direction = player.GetFacingRight() ? -1 : 1;
            float jumping = 1.0f - ((collisionCarryPlayer - 0.6f) / 1.2f);
            float height = Mathf.Min(collisionCarryPlayer, collisionCarry);

            destination = new Vector2(
                canMoveX ? player.GetComponent<Rigidbody2D>().position.x + (carryWidth * direction * jumping): m_Rigidbody.position.x,
                canMoveY ? player.GetComponent<Rigidbody2D>().position.y + height - 0.6f: m_Rigidbody.position.y);
            m_Rigidbody.position = Vector2.SmoothDamp(m_Rigidbody.position, destination, ref m_Velocity, pickupSmooth * Time.deltaTime);
        }
    }

    private void Carry(bool teleporting = false)
    {
        if (isCarried)
        {
            m_Rigidbody.velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x * 2f, m_Rigidbody.velocity.y);
            throwStartHeight = player.GetComponent<Rigidbody2D>().position.y + carryHeight;
            isThrown = true;
            canExtendThrow = true;
        }
        isCarried = !isCarried;
        canCarry = false;
    }

    private void Drop()
    {
        m_Rigidbody.velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, m_Rigidbody.velocity.y);
        isCarried = false;
        canCarry = false;
    }
}
