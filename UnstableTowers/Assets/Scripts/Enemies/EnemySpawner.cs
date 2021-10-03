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

    public void Initialize(WorldNode _n) {
        node = _n;
    }

    public void SpawnEnemies(Transform parent, float points, List<EnemyManager.EnemyInfo> info) {
        List<EnemyManager.EnemyInfo> tmpInfo = new List<EnemyManager.EnemyInfo>(info);

        //Select Random Enemies to spawn, as long a there are enough Points for it
        while (points > 0 && tmpInfo.Count > 0) {
            var idx = Random.Range(0, tmpInfo.Count);
            if (tmpInfo[idx].cost <= points) {
                var enemy = Instantiate(info[idx].prefab, parent);
                enemy.GetComponent<Enemy>().initialize(pathToReactor);
                enemy.transform.position = transform.position + Vector3.up; //TODO: Randomize 
                points -= tmpInfo[idx].cost;
            } else {
                //If this Enemy is to expensive, remove it from the selection proccess
                tmpInfo.RemoveAt(idx);
            }
        }
    }

    private void Start() {
        pathfinder = FindObjectOfType<WorldPathfinder>();
        Assert.IsNotNull(pathfinder);
        manager = FindObjectOfType<WorldManager>();
        Assert.IsNotNull(manager);
        Assert.IsNotNull(node, "Initialize was not called");
        pathToReactor = pathfinder.FindPathFromTo(node, manager.reactorNode);
    }
}
