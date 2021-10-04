using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTower : Tower
{
    public GameObject rotation;

    private new void Start() {
        base.Start();
    }

    private void Update() {
        CheckTarget();
        if(target != null) {

        }
    }
}
