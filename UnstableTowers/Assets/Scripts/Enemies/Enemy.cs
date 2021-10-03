using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Enemy : MonoBehaviour
{
    public float minSpeed, maxSpeed;
    private float speed;
    public float heightOffset;
    public float nodeDistance;
    private WorldPath path = null;
    private int pathIndex;

    private Vector3 getDirectionTo(WorldNode node) {
        Vector3 ret = (node.wayPoint.transform.position - transform.position);
        return ret;
    }

    public void Update() {
        if(path == null) {
            return;
        }
        Vector3 direction = getDirectionTo(path.nodes[pathIndex]);
        if(direction.magnitude < nodeDistance) {
            pathIndex += 1;
            if(pathIndex < path.nodes.Count) {
                direction = getDirectionTo(path.nodes[pathIndex]);
            } else {
                Destroy(gameObject);
            }
        }
        direction = (direction.normalized * speed * Time.deltaTime);
        this.transform.Translate(direction, Space.World);
    }

    public void initialize (WorldPath _p) {
        speed = Random.Range(minSpeed, maxSpeed);
        pathIndex = 1;  //Spawner itself is on 0
        path = _p;
    }
}
