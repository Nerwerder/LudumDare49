using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float cruisingAltitude = 3f;
    public float arrivalDistance = 1f;

    private WorldNode target = null;

    private void MovetoTarget() {
        var direction = (target.wayPoint.transform.position + Vector3.up * cruisingAltitude) - transform.position;
        if (direction.magnitude < arrivalDistance) {
            target = null;
        } else {
            transform.Translate(direction.normalized * Time.deltaTime * movementSpeed);
        }
    }

    void Update() {
        if (target != null) { MovetoTarget(); }
    }

    public void SetTarget(WorldNode _t) {
        target = _t;
        Debug.Log("Set Player Target: " + target.empty.name);
    }
}
