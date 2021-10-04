using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTower : Tower
{
    public GameObject cannonMount; //Rotate arount Y
    //public GameObject cannon;      //Rotate Up and Down

    private new void Start() {
        base.Start();
    }

    private void RotateToTarget() {
        //TODO: Rotation
        var enemyDirection = (target.transform.position - transform.position);
        enemyDirection.y = 0;
        cannonMount.transform.forward = -enemyDirection.normalized;
        cannonMount.transform.Rotate(0, 0, -90);
    }

    private void AttackTarget() {
        coolDownTimer -= Time.deltaTime;
        if(coolDownTimer <= 0) {
            //TODO: Animation
            if(target.Damage(damage)) {
                target = null;
            }
            coolDownTimer = coolDown;
        }
    }

    private void Update() {
        CheckTarget();
        if(target != null) {
            RotateToTarget();
            AttackTarget();
        }
    }
}
