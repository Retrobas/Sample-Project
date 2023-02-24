using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask m_groundLayer;
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_jumpHeight;
    [SerializeField] private AudioClip m_jumpSound;

    private Rigidbody2D m_rigid;
    private float m_jumpSpeed;
    private float m_wallDir;

    private bool m_isAir = false;
    private bool m_isWall = false;
    private bool m_isWallJump = false;
    private readonly WaitForSeconds m_wallWaitTime = new(0.5f);


    private void Start()
    {
        m_rigid = GetComponent<Rigidbody2D>();

        // mGh = (0.5)mv^2
        m_jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * m_rigid.gravityScale * m_jumpHeight);
    }

    void Update()
    {
        if (m_isWallJump) return;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        // Movement
        m_rigid.velocity = new Vector2(horizontal * m_moveSpeed, m_rigid.velocity.y);

        // 방향전환
        if (Mathf.Abs(horizontal) > 0.01f)
        {
            transform.localRotation = Quaternion.Euler(0f, horizontal > 0f ? 0f : 180f, 0f);
        }

        // Wall Movement
        if (m_isAir && m_isWall && Mathf.Abs(horizontal - transform.forward.z) < 0.01f)
        {
            m_rigid.velocity = new Vector2(m_rigid.velocity.x, -1f);
        }

        if (vertical > 0.001f)
        {
            // Jump
            if (!m_isAir)
            {
                SoundManager.Instance.PlaySE(m_jumpSound);
                m_rigid.velocity += new Vector2(0, m_jumpSpeed);
                m_isAir = true;
            }

            // Wall Jump
            else if (m_isWall && Mathf.Abs(m_wallDir - horizontal) < 0.01f)
            {
                m_isWall = false;
                m_isWallJump = true;
                transform.localRotation = Quaternion.Euler(0f, horizontal > 0f ? 180f : 0f, 0f);
                StartCoroutine(WallCoroutine(horizontal));
            }
        }
    }

    private void FixedUpdate()
    {
        // Check Ground
        var groundHit = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - 0.4f),
            0.49f, m_groundLayer);

        if (!m_isAir && !groundHit)
        {
            m_isAir = true;
        }

        else if (m_isAir && m_rigid.velocity.y <= 0f && groundHit)
        {
            m_isAir = false;
        }

        // Check Wall
        var wallHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y),
            new Vector2(transform.forward.z, transform.forward.y), 0.6f, m_groundLayer);

        if (m_isAir && wallHit && !m_isWallJump && m_rigid.velocity.y <= 0f)
        {
            m_isWall = true;
            m_wallDir = transform.forward.z;
        }
        else
        {
            m_isWall = false;
        }
    }

    private IEnumerator WallCoroutine(float horizontal)
    {
        SoundManager.Instance.PlaySE(m_jumpSound);
        m_rigid.velocity = new Vector2(m_jumpSpeed * horizontal * -0.5f, m_jumpSpeed * 0.5f * Mathf.Pow(3f, 0.5f));
        yield return m_wallWaitTime;
        m_isWallJump = false;
    }
}
