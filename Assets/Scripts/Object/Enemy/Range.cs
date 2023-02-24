using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : Enemy
{
    [Header("���Ÿ� ���� ����")]
    [SerializeField] private GameObject m_bulletPrefab;
    [SerializeField] private float m_bulletSpeed;

    protected override IEnumerator AttackCoroutine(RaycastHit2D hit)
    {
        Bullet bullet = Instantiate(m_bulletPrefab, transform.position, transform.rotation).GetComponent<Bullet>();
        bullet.Speed = m_bulletSpeed;
        bullet.Damage = m_damage;
        yield return base.AttackCoroutine(hit);
    }
}
