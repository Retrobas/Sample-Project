using System.Collections;
using UnityEngine;

[System.Serializable]
public abstract class AttackableObject : MonoBehaviour
{
    protected abstract IEnumerator AttackCoroutine(RaycastHit2D hit);

    [Header("HP")]
    [SerializeField] private float m_maxHp;
    private float m_hp;

    [Header("공격")]
    [SerializeField] protected float m_damage;
    [SerializeField] protected float m_attackRange;
    [SerializeField] protected float m_attackDelay;
    protected bool m_canAttack = true;

    [Header("피격")]
    [SerializeField] protected float m_attackedDelay = 0.2f;
    protected bool m_canAttacked = true;

    public float MaxHp { get => m_maxHp; set => m_maxHp = value < 0f ? 0f : value; }

    public float Hp { get => m_hp; set => m_hp = value < 0f ? 0f : (value > m_maxHp ? m_maxHp : value); }


    protected virtual void Awake()
    {
        m_hp = m_maxHp;
    }

    public void Attack(LayerMask mask)
    {
        if (!m_canAttack) return;

        // 공격 감지
        var hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y),
            new Vector2(transform.forward.z, transform.forward.y), m_attackRange, mask);
        if (!hit) return;

        m_canAttack = false;
        StartCoroutine(AttackCoroutine(hit));
    }

    public void Attacked(float damage)
    {
        if (!m_canAttacked) return;
        m_canAttacked = false;
        Hp -= damage;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer is not null)
        {
            StartCoroutine(AttackedCoroutine(spriteRenderer));
        }
    }

    protected virtual IEnumerator AttackedCoroutine(SpriteRenderer spriteRenderer)
    {
        float alpha = 1f / (m_attackedDelay * 0.5f);
        for (int i = 0; i < 2; ++i)
        {
            while (spriteRenderer.color.a > 0.01f)
            {
                spriteRenderer.color -= new Color(0f, 0f, 0f, alpha * Time.deltaTime);
                yield return null;
            }
            while (spriteRenderer.color.a < 0.99f)
            {
                spriteRenderer.color += new Color(0f, 0f, 0f, alpha * Time.deltaTime);
                yield return null;
            }
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        m_canAttacked = true;
    }
}
