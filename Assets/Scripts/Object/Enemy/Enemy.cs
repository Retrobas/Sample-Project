using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : AttackableObject
{
    protected override IEnumerator AttackCoroutine(RaycastHit2D hit)
    {
        yield return new WaitForSeconds(m_attackDelay);
        m_canAttack = true;
    }

    [Header("AI 설정")]
    [SerializeField] protected float m_wanderSpeed;

    [Header("마스크 설정")]
    [SerializeField] protected LayerMask m_groundMask;
    [SerializeField] protected LayerMask m_playerMask;

    public bool IsDead => Hp < 0.001f;

    protected Rigidbody2D m_rigid;
    private bool m_mustStop = false;
    private bool m_isActing;

    protected override void Awake()
    {
        base.Awake();
        m_rigid = GetComponent<Rigidbody2D>();
        StartCoroutine(EnemyAI());
    }

    private void FixedUpdate()
    {
        Attack(m_playerMask);
        
        // Check Ground
        var hit = Physics2D.Raycast(new Vector2(transform.position.x + transform.forward.z, transform.position.y)
            , Vector2.down, 1f, m_groundMask);

        if (!hit)
        {
            m_mustStop = true;
        }
    }

    private IEnumerator EnemyAI()
    {
        while (true)
        {
            m_isActing = true;
            StartCoroutine(WanderCoroutine());
            yield return new WaitUntil(() => !m_isActing && m_canAttack);

            m_isActing = true;
            StartCoroutine(RotationAfterRestCoroutine());
            yield return new WaitUntil(() => !m_isActing);
        }
    }

    private IEnumerator RotationAfterRestCoroutine()
    {
        float waitingTime = Random.Range(0.3f, 2f);
        while (waitingTime > 0f)
        {
            if (!m_canAttack) break;
            waitingTime -= Time.deltaTime;
            yield return null;
        }
        if (m_canAttack)
        {
            transform.Rotate(0f, 180f, 0f);
            m_mustStop = false;
        }
        m_isActing = false;
    }

    private IEnumerator WanderCoroutine()
    {
        float wanderTime = Random.Range(3f, 5f);
        float speed = transform.forward.z > 0f ? m_wanderSpeed : -m_wanderSpeed;
        while (wanderTime > 0f)
        {
            if (m_mustStop || !m_canAttack) break;
            m_rigid.velocity = new Vector2(speed, m_rigid.velocity.y);
            wanderTime -= Time.deltaTime;
            yield return null;
        }
        m_isActing = false;
    }
}
