using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTower : Tower
{
    public GameObject cannonMount; //Rotate arount Y
    public GameObject gunEffect;

    protected override void RotateToTarget() {
        var enemyDirection = (target.transform.position - transform.position);
        enemyDirection.y = 0;
        cannonMount.transform.forward = -enemyDirection.normalized;
        cannonMount.transform.Rotate(0, 0, -90);
    }

    protected override void AttackTarget() {
        gunEffect.GetComponent<ParticleSystem>().Play();
        if(target.Damage(damage)) {
            target = null;
        }
    }
}
