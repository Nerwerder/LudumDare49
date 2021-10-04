using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Pump : MonoBehaviour
{
    private Reactor reactor;
    public float water;
    public float minWater;

    private void Start() {
        reactor = FindObjectOfType<Reactor>();
        Assert.IsNotNull(reactor);
        reactor.water = water;
    }

    public void openValve() {
        water *= 1.1f;
        reactor.water = water;
    }

    public void closeValve() {
        water /= 1.1f;
        if(water < minWater) {
            water = minWater;
        }
        reactor.water = water;
    }
}
