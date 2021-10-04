using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTower : Tower
{
    public GameObject cannonMount; //Rotate arount Y
    //public GameObject cannon;      //Rotate Up and Down

    protected override void RotateToTarget() {
        var enemyDirection = (target.transform.position - transform.position);
        enemyDirection.y = 0;
        cannonMount.transform.forward = -enemyDirection.normalized;
        cannonMount.transform.Rotate(0, 0, -90);
    }

    protected override void AttackTarget() {
        if(coolDownTimer <= 0) {
            //TODO: Animation
            if(target.Damage(damage)) {
                target = null;
            }
            coolDownTimer = coolDown;
        }
    }


}
