using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactor : Structure
{
    public float water { set; get; }
    public float power { set; get; }

    public float maxTemperature;
    public float extremeTmpMax;
    public float minTemperature;
    public float extremeTmpMin;
    public float temperature;
    public float tmpChangeFactor;
    public float everythingOKPower;

    private void Update() {
        var tmpChange = ((power / water) - 1) * tmpChangeFactor * Time.deltaTime;
        temperature += tmpChange;
    }

    public float GetPower() {
        if (temperature > maxTemperature) {
            if(temperature < extremeTmpMax) {
                //maxTemperature(200) <- temperature -> extremeTmp(300)
                var f = 1-((temperature - maxTemperature) / (extremeTmpMax - maxTemperature));
                var pwr = power * f;
                Debug.Log("Power(max): " + f);
                return pwr;
            } else {
                return 0;
            }
        } else if (temperature < minTemperature) {
            if(temperature > extremeTmpMin) {
                var f = ((temperature - extremeTmpMin) / (minTemperature - extremeTmpMin));
                var pwr = power * f;
                Debug.Log("Power(min): " + f);
                return pwr;
            } else {
                return 0f;
            }
        } else {
            return power;
        }
    }

    public float GetTemperature() {
        return temperature;
    }

    public bool IsInSafetyMode() {
        return (power <= everythingOKPower);
    }
}
