using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruction : MonoBehaviour
{
    public float seconds;
    void Update()
    {
        seconds -= Time.deltaTime;
        if(seconds <= 0) {
            Destroy(gameObject);
        }
    }
}
