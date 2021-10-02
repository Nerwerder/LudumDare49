using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemySpawner : MonoBehaviour
{
    public WorldNode node { get; set; }
    private WorldPathfinder pathfinder;
    private WorldManager manager;
    private WorldPath pathToReactor;

    public void initialize(WorldNode _n) {
        node = _n;
    }

    public void spawnEnemies(Transform parent, float points, List<EnemyManager.EnemyInfo> info) {
        var enemy = Instantiate(info[0].prefab, parent);
        enemy.transform.position = transform.position + Vector3.up;
    }

    private void Start() {
        pathfinder = FindObjectOfType<WorldPathfinder>();
        Assert.IsNotNull(pathfinder);
        manager = FindObjectOfType<WorldManager>();
        Assert.IsNotNull(manager);

        pathToReactor = pathfinder.findPathFromTo(node, manager.reactorNode);
    }
}
