using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactor : Structure
{
    public float water { set; get; }
    public float power { set; get; }

    public float maxTemperature;
    public float minTemperature;
    public float temperature;
    public float tmpChangeFactor;

    private void Update() {
        var tmpChange = ((power / water) - 1) * tmpChangeFactor * Time.deltaTime;
        temperature += tmpChange;
    }

    public float GetPower() {
        if (temperature > maxTemperature || temperature < minTemperature) {
            return 0f;
        } else {
            return power;
        }
        
    }

    public float GetTemperature() {
        return temperature;
    }
}
