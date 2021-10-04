using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Pump : MonoBehaviour
{
    private Reactor reactor;
    public float water;

    private void Start() {
        reactor = FindObjectOfType<Reactor>();
        Assert.IsNotNull(reactor);
    }

    public void openValve() {
        water *= 1.1f;
    }

    public void closeValve() {
        water /= 1.1f;
    }
}
