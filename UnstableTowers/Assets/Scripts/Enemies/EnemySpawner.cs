using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemySpawner : MonoBehaviour
{
    public WorldNode node { get; set; }
    private WorldPathfinder pathfinder;
    private WorldManager worldManager;
    private WorldPath pathToReactor;
    public EnemyManager enemyManager { get; set; }

    public void Initialize(WorldNode _n) {
        node = _n;
    }

    public void SpawnEnemies(Transform parent, float points, List<Enemy> spawnables) {
        List<Enemy> tmpInfo = new List<Enemy>(spawnables);
        //Select Random Enemies to spawn, as long a there are enough Points for it
        while (points > 0 && tmpInfo.Count > 0) {
            var idx = Random.Range(0, tmpInfo.Count);
            if (tmpInfo[idx].cost <= points) {
                var enemy = Instantiate(spawnables[idx], parent);
                enemy.GetComponent<Enemy>().Initialize(worldManager, enemyManager, pathToReactor);
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
        worldManager = FindObjectOfType<WorldManager>();
        Assert.IsNotNull(worldManager);
        Assert.IsNotNull(node, "Initialize was not called");
        pathToReactor = pathfinder.FindPathFromTo(node, worldManager.reactorNode);
    }
}
