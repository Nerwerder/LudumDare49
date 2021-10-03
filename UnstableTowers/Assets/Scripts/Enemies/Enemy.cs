using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    private WorldPath path = null;
    private EnemyManager enemyManager;
    private WorldManager worldManager;
    private int pathIndex;

    private Vector3 GetDirectionTo(WorldNode node) {
        Vector3 ret = (node.wayPoint.transform.position - transform.position);
        return ret;
    }

    private void Die() {
        worldManager.metal += loot;
        Destroy(gameObject);
        enemyManager.DeRegisterEnemy(this);
    }

    public bool Attack(float damage) {
        hp -= damage;
        return (hp > 0);
    }

    public void Update() {
        if(hp <= 0) {
            Die();
        }
        if(path == null) {
            return;
        }
        Vector3 direction = GetDirectionTo(path.nodes[pathIndex]);
        if(direction.magnitude < nodeDistance) {
            pathIndex += 1;
            if(pathIndex < path.nodes.Count) {
                direction = GetDirectionTo(path.nodes[pathIndex]);
            } else {
                var s = path.GetEnd().structure;
                if(s != null) {
                    s.GetComponent<Structure>().Attack(damage);
                }
                Die();
            }
        }
        direction = (direction.normalized * speed * Time.deltaTime);
        this.transform.Translate(direction, Space.World);
    }

    public void Initialize (WorldManager _w, EnemyManager _e, WorldPath _p) {
        speed = Random.Range(minSpeed, maxSpeed);
        pathIndex = 1;  //Spawner itself is on 0
        path = _p;
        enemyManager = _e;
        worldManager = _w;
        enemyManager.RegisterEnemy(this);
    }
}
