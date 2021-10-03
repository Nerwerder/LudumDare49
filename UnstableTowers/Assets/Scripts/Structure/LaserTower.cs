using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LaserTower : Tower
{
    private Enemy curTarget = null;
    private EnemyManager enemyManager;
    public GameObject dish;

    private void Start() {
        enemyManager = FindObjectOfType<EnemyManager>();
        Assert.IsNotNull(enemyManager);
        Assert.IsNotNull(dish);
    }

    private void Update() {
        //Try to get a new Target
        if(curTarget == null || curTarget.gameObject == null || ((curTarget.transform.position - transform.position).magnitude < range)) {
            curTarget = enemyManager.GetEnemyInRange(transform.position, range);
        }
        if(curTarget != null) {
            //Turn to enemy
            var enemyDirection = (curTarget.transform.position - transform.position);
            enemyDirection.y = 0;
            dish.transform.right = enemyDirection.normalized;
            if (curTarget.Attack(damage * Time.deltaTime)) {    //Attack returns true if hp<0
                curTarget = null;
            }
        }
    }
}
