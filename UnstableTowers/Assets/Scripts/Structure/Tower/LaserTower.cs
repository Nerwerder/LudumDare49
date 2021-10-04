using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LaserTower : Tower
{
    public GameObject dish;

    private new void Start() {
        base.Start();
        Assert.IsNotNull(dish);
    }

    private void Update() {
        CheckTarget();
        if(target != null) {
            //Turn to enemy
            var enemyDirection = (target.transform.position - transform.position);
            enemyDirection.y = 0;
            dish.transform.right = enemyDirection.normalized;
            if (target.Damage(damage * Time.deltaTime)) {
                target = null;
            }
        }
    }
}
