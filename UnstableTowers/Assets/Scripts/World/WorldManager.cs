using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    List<WorldPath> paths = new List<WorldPath>();
    Dictionary<WorldPath, LineRenderer> pathRenderer = new Dictionary<WorldPath, LineRenderer>();
    public bool drawPaths;
    public Text pointsMessage;
    public Text metalMessage;
    public Text healthMessage;
    public Text tmpMessage;
    public int metal;
    public int points;

    public float messageUpdateTick;
    private float updateTimer;

    //PowerProduction
    public Text powerProductionMessage;
    public Reactor reactor { get; set; }
    public WorldNode reactorNode { get; set; }

    //PowerConsumption
    public Text powerConsumptionMessage;
    public List<Tower> towers {get; set;}

    //Ratio
    public Text ratioMessage;
    public float ratio { get; set; }
    public float minTowerRatioTolerance;
    public float maxTowerRatioTolerance;


    private void Start() {
        towers = new List<Tower>();
        ratio = 1f;
    }

    /// <summary>
    /// Special Mode for the Beginning of the Game (Power <= 25 and ratio >= 0.8)
    /// </summary>
    /// <returns></returns>
    private bool PowerIsSafe() {
        return (reactor.IsInSafetyMode() && ratio >= minTowerRatioTolerance);
    }

    private bool RatioInTolerance() {
        return (ratio > minTowerRatioTolerance && ratio < maxTowerRatioTolerance);
    }

    public float GetTowerRatio() {
        if (RatioInTolerance() || PowerIsSafe()) {
            return 1f;
        } else {
            var ts = (1 - Mathf.Abs(ratio - 1f));
            return (ts > 0f) ? (ts) : (0f);
        }
    }

    public void RegisterPath(WorldPath _p) {
        Assert.IsFalse(pathRenderer.ContainsKey(_p));
        GameObject empty = new GameObject("LineRendererParent");
        empty.transform.parent = transform;
        LineRenderer lineRenderer = empty.AddComponent<LineRenderer>();
        //lineRenderer.material = Material; //TODO
        lineRenderer.widthMultiplier = 0.2f;
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
        paths.Add(_p);
        pathRenderer.Add(_p, lineRenderer);
    }

    public void DeregisterPath(WorldPath _p) {
        Assert.IsTrue(pathRenderer.ContainsKey(_p));
        Destroy(pathRenderer[_p]);
        pathRenderer.Remove(_p);
        paths.Remove(_p);
    }

    public void DrawPath(WorldPath _p) {
        if(drawPaths) {
            Assert.IsTrue(pathRenderer.ContainsKey(_p));
            var renderer = pathRenderer[_p];
            renderer.positionCount = _p.nodes.Count;
            var points = new Vector3[_p.nodes.Count];
            for (int k = 0; k < _p.nodes.Count; ++k) {
                points[k] = _p.nodes[k].wayPoint.transform.position;
            }
            renderer.SetPositions(points);
        }
    }

    public void UpdateAllPaths() {
        foreach(var p in paths) {
            p.Update();
        }
    }

    private float GetTowerPowerConsumption() {
        float pc = 0f;
        foreach (var t in towers) {
            pc += t.currentPowerUsage;
        }
        return pc;
    }

    private Color GetRatioColor() {
        Color c = Color.black;
        if (PowerIsSafe()) {
            return Color.gray;
        } else if (ratio > 1.6 || ratio < 0.4) {
            return Color.red;
        } else if (RatioInTolerance()) {
            return Color.green;
        } else {
            return Color.yellow;
        }
    }


    public void Update() {
        updateTimer += Time.deltaTime;
        if(updateTimer >= messageUpdateTick) {
            //Update Messages
            pointsMessage.text    = "Points: " + points;
            metalMessage.text     = "Metal:\n" + metal;
            healthMessage.text    = "Reactor:\n" + reactor.GetHp();

            tmpMessage.text       = "Temperatur:\n" + reactor.GetTemperature().ToString("0.#");
            tmpMessage.color = reactor.GetTemperaturColor();

            powerProductionMessage.text = "Production:\n" + reactor.GetPower().ToString("0.#") + "\nPower";
            if(reactor.GetTemperaturOutOfBounds()) {
                powerProductionMessage.color = reactor.GetTemperaturColor();
            }

            float pc = GetTowerPowerConsumption();
            powerConsumptionMessage.text = "Consumption:\n" + pc.ToString("0.#") + "\nPower";
            ratio = reactor.GetPower() / pc;
            ratioMessage.text = "Ratio:\n" + ((ratio == float.NaN || ratio == float.PositiveInfinity) ? ("---") : (ratio.ToString("0.##")));
            ratioMessage.color = GetRatioColor();
            updateTimer = 0f;
        }
    }
}
