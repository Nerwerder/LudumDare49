using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Tower : Structure
{
    public float minRange;
    public float maxRange;
    public float damage;
    public float coolDown;
    private float coolDownTimer;
    public float basicPowerConsumption;
    public float activePowerConsumption;
    public float currentPowerUsage { set; get; }

    protected Enemy target = null;
    protected EnemyManager enemyManager;
    protected WorldManager worldManager;

    protected void Start() {
        enemyManager = FindObjectOfType<EnemyManager>();
        Assert.IsNotNull(enemyManager);
        worldManager = FindObjectOfType<WorldManager>();
        Assert.IsNotNull(worldManager);
        coolDownTimer = coolDown;
        currentPowerUsage = 0f;
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
    protected bool CheckTarget() {
        if (!(ValidTarget() && TargetInRange())) {
            target = enemyManager.GetEnemyInRange(transform.position, minRange, maxRange);
        }
        return (target != null);
    }

    protected void CalculatePowerconsumption() {
        if (coolDownTimer < coolDown) {
            currentPowerUsage = basicPowerConsumption + activePowerConsumption;
            coolDownTimer += Time.deltaTime * worldManager.GetTowerRatio();
        } else {
            currentPowerUsage = basicPowerConsumption;
        }
    }

    protected abstract void RotateToTarget();
    protected abstract void AttackTarget();

    private void Update() {
        CalculatePowerconsumption();
        if (CheckTarget()) {
            if (worldManager.GetTowerRatio() != 0) {
                RotateToTarget();
            }
            if (coolDownTimer >= coolDown) {
                AttackTarget();
                coolDownTimer = 0f;
            }
        }
    }

    public override void Remove() {
        worldManager.towers.Remove(this);
        base.Remove();
    }
}
