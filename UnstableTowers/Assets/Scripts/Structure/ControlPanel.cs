using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ControlPanel : MonoBehaviour
{
    private Reactor reactor;
    public float power;
    public float minPower;

    void Start()
    {
        reactor = FindObjectOfType<Reactor>();
        Assert.IsNotNull(reactor);
        reactor.power = power;
    }

    public void PowerUp() {
        power *= 1.1f;
        reactor.power = power;
    }

    public void PowerDown() {
        power /= 1.1f;
        if(power < minPower) {
            power = minPower;
        }
        reactor.power = power;
    }
}
