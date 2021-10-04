using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTower : Tower
{
    public GameObject mortarBase;
    public GameObject mortarExplosion;
    public float damageRadius;

    protected override void RotateToTarget() {
        var enemyDirection = (target.transform.position - transform.position);
        enemyDirection.y = 0;
        mortarBase.transform.forward = enemyDirection.normalized;
        mortarBase.transform.Rotate(-90, 0, 0);
    }

    protected override void AttackTarget() {
        if (coolDownTimer <= 0) {
            //Get all Enemies in in the impactZone
            var ens = enemyManager.GetEnemiesInRadius(target.transform.position, damageRadius);
            //Create the Explosion
            var explosion = Instantiate(mortarExplosion, enemyManager.explosionParent);
            explosion.transform.position = target.transform.position;
            //Damage all Enemies
            foreach(var e in ens) {
                e.Damage(damage);
            }
            coolDownTimer = coolDown;
        }
    }
}
