using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Enemy
{
    protected override IEnumerator AttackCoroutine(RaycastHit2D hit)
    {
        if (hit)
        {
            var player = hit.collider.GetComponent<Player>();
            player.Attacked(m_damage);
            yield return base.AttackCoroutine(hit);
        }
    }
}
