using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public float hp;
    public float minSpeed, maxSpeed;
    private float speed;
    public float heightOffset;
    public float nodeDistance;
    public float cost;
    public int loot;
    public int damage;
    public int points;
    private WorldPath path = null;
    private EnemyManager enemyManager;
    private WorldManager worldManager;
    private int pathIndex;

    private bool resolvePathChange = false;     //Resolve the Path Change AFTER the path changed
    private WorldPath tmpPath = null;           //Used to store a temporary local copy of the path

    private void Die() {
        worldManager.metal += loot;
        worldManager.points += points;
        enemyManager.DeRegisterEnemy(this);
        if(path != null) {
            path.enemies.Remove(this);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Damage the Enemy
    /// </summary>
    /// <param name="damage">How much Damage to do</param>
    /// <returns>True if the Enemy was Destroyed</returns>
    public bool Damage(float damage) {
        hp -= damage;
        if(hp <= 0) {
            Die();
            return true;
        }
        return false;
    }

    private void LeavePath() {
        path.enemies.Remove(this);
        path = null;
    }

    public void PreparePathUpdate() {
        tmpPath = new WorldPath(path);
        resolvePathChange = true;
    }

    private void ResolvePathChange() {
        //Iterate over both Paths and check if the currentIndex is still on Point where we can keep walking on the new Path
        var maxIdx = Math.Min(path.nodes.Count, tmpPath.nodes.Count);
        int idx;
        for(idx = 0; idx < maxIdx; ++idx) {
            if (idx > pathIndex) {
                break;
            }
            if (path.nodes[idx].id != tmpPath.nodes[idx].id) {
                break;
            }
        }
        if(idx < pathIndex) {
            LeavePath();
        } else {
            tmpPath = null;
        }
    }

    private void FollowPath() {
        //Which Path to use (the "old" one or the current)?
        WorldPath pth;
        if(resolvePathChange) {
            resolvePathChange = false;
            ResolvePathChange(); 
        }
        pth = (path != null) ? (path) : (tmpPath);
        if (pth == null) { return; }   //Chicken/Egg

        //Get TargetNode
        WorldNode tn;
        tn = pth.nodes[pathIndex];
        //Check Distance to TargetNode
        Vector3 direction = tn.GetDirectionToWaypoint(transform.position);
        //Are we near enough to interact with the Node?
        if(direction.magnitude < nodeDistance) {
            //There is a structure
            if(tn.structure != null) {
                tn.structure.GetComponent<Structure>().Damage(damage);
                Die();  //TODO: will all Monsterd Die after Attacking?
                return;
            } else {
                //Nothing to do, get the next Node
                if((pathIndex +1)< pth.nodes.Count) {
                    tn = pth.nodes[++pathIndex];
                }
            }
        }
        //Move to Node
        direction = (direction.normalized * speed * Time.deltaTime);
        this.transform.Translate(direction, Space.World);
    }

    public void Update() {
        FollowPath();
    }

    public void Initialize (WorldManager _w, EnemyManager _e, WorldPath _p) {
        speed = Random.Range(minSpeed, maxSpeed);
        pathIndex = 1;  //Spawner itself is on 0
        path = _p;
        path.enemies.Add(this);
        enemyManager = _e;
        worldManager = _w;
        enemyManager.RegisterEnemy(this);
    }
}
