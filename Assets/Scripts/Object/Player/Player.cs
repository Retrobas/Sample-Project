using System.Collections;
using System.Text;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class Player : AttackableObject
{
    [Header("마스크 설정")]
    [SerializeField] private LayerMask m_enemyMask;

    [Header("UI 설정")]
    [SerializeField] private TextMeshProUGUI m_hpText;
    private readonly StringBuilder m_stringBuilder = new(4);

    protected override void Awake()
    {
        base.Awake();
        ChangeHpText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Attack(m_enemyMask);
        }
    }

    protected override IEnumerator AttackCoroutine(RaycastHit2D hit)
    {
        if (hit)
        {
            var enemy = hit.collider.GetComponent<Enemy>();
            enemy.Attacked(m_damage);
            if (enemy.IsDead) Destroy(enemy.gameObject);
        }
        yield return new WaitForSeconds(m_attackDelay);
        m_canAttack = true;
    }

    protected override IEnumerator AttackedCoroutine(SpriteRenderer spriteRenderer)
    {
        ChangeHpText();
        if (Hp > 0.1f)
        {
            yield return base.AttackedCoroutine(spriteRenderer);
        }
        else
        {
            SceneController.Instance.RestartScene();
            yield return null;
        }
    }

    private void ChangeHpText()
    {
        m_stringBuilder.Clear();
        m_stringBuilder.Append("Hp: ");
        m_stringBuilder.Append((int)Hp);
        m_stringBuilder.Append(" / ");
        m_stringBuilder.Append((int)MaxHp);
        m_hpText.text = m_stringBuilder.ToString();
    }
}
