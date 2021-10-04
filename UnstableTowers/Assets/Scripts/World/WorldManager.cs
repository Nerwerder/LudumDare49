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
    public float minTowerRatio;
    public float maxTowerRatio;


    private void Start() {
        towers = new List<Tower>();
        ratio = 1f;
    }

    public bool TowersWorking() {
        return (ratio > minTowerRatio && ratio < maxTowerRatio);
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

    public void Update() {
        updateTimer += Time.deltaTime;
        if(updateTimer >= messageUpdateTick) {
            //Update Messages
            pointsMessage.text    = "Points:   " + points;
            metalMessage.text     = "Metal:    " + metal;
            healthMessage.text    = "Reactor:  " + reactor.GetHp();
            tmpMessage.text       = "Tmp:      " + reactor.GetTemperature().ToString("0.#");
            powerProductionMessage.text = "PP: " + reactor.GetPower().ToString("0.#");
            float pc = GetTowerPowerConsumption();
            powerConsumptionMessage.text = "PC: " + pc.ToString("0.#");
            ratio = reactor.GetPower() / pc;
            ratioMessage.text = "Ratio: " + ((ratio == float.NaN || ratio == float.PositiveInfinity) ? ("---") : (ratio.ToString("0.##")));
            updateTimer = 0f;
        }
    }
}
