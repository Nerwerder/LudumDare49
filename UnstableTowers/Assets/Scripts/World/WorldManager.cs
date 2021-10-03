using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    Dictionary<WorldPath, LineRenderer> pathRenderer = new Dictionary<WorldPath, LineRenderer>();
    public bool drawPaths;
    public Text statusMessage;
    public int metal;

    public Reactor reactor { get; set; }
    public WorldNode reactorNode { get; set; }

    public void RegisterPath(WorldPath _p) {
        Assert.IsFalse(pathRenderer.ContainsKey(_p));
        GameObject empty = new GameObject("LineRendererParent");
        empty.transform.parent = transform;
        LineRenderer lineRenderer = empty.AddComponent<LineRenderer>();
        //lineRenderer.material = Material; //TODO
        lineRenderer.widthMultiplier = 0.4f;
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
        pathRenderer.Add(_p, lineRenderer);
    }

    public void DeregisterPath(WorldPath _p) {
        Assert.IsTrue(pathRenderer.ContainsKey(_p));
        Destroy(pathRenderer[_p]);
        pathRenderer.Remove(_p);
    }

    public void DrawPath(WorldPath _p) {
        Assert.IsTrue(pathRenderer.ContainsKey(_p));
        var renderer = pathRenderer[_p];
        renderer.positionCount = _p.nodes.Count;
        var points = new Vector3[_p.nodes.Count];
        for(int k = 0; k < _p.nodes.Count; ++k) {
            points[k] = _p.nodes[k].wayPoint.transform.position;
        }
        renderer.SetPositions(points);
    }


    public void Update() {
        statusMessage.text = "Metal: " + metal + '\n' + "Reactor: " + reactor.hp;
    }
}
