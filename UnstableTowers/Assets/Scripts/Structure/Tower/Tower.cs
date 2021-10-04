using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Tower : Structure
{
    public float minRange;
    public float maxRange;
    public float damage;
    public float coolDown;
    public float coolDownTimer;
    protected Enemy target = null;
    protected EnemyManager enemyManager;

    protected void Start() {
        enemyManager = FindObjectOfType<EnemyManager>();
        Assert.IsNotNull(enemyManager);
        coolDownTimer = 0f;
    }

    protected bool ValidTarget() {
        return (target != null && target.gameObject != null);
    }

    protected bool TargetInRange() {
        Assert.IsNotNull(target);
        var d = target.transform.position - transform.position;
        return (d.magnitude > minRange && d.magnitude < maxRange);
    }

    //Try to get a Target in Range (if the current target is valid and still in range, keep it)
    protected void CheckTarget() {
        if(!(ValidTarget() && TargetInRange())) {
            target = enemyManager.GetEnemyInRange(transform.position, minRange, maxRange);
        }
    }
}
