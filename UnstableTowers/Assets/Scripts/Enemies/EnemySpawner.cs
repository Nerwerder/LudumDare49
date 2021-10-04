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
    public float coolDown;
    private float coolDownTimer;

    //TMP
    private Transform spawnParent;
    private List<Enemy> tmpInfo;
    private float points = 0f;

    public void Initialize(WorldNode _n) {
        node = _n;
    }

    public void SpawnEnemies(Transform _parent, float _points, List<Enemy> _spawnables) {
        spawnParent = _parent;
        tmpInfo = new List<Enemy>(_spawnables);
        points += _points;
    }

    private void Update() {
        coolDownTimer += Time.deltaTime;
        if(coolDownTimer > coolDown) {
            //Select Random Enemies to spawn, as long a there are enough Points for it
            if ((points > 0) && (tmpInfo.Count > 0)) {
                var idx = Random.Range(0, tmpInfo.Count);
                if (tmpInfo[idx].cost <= points) {
                    var enemy = Instantiate(tmpInfo[idx], spawnParent);
                    enemy.GetComponent<Enemy>().Initialize(worldManager, enemyManager, pathToReactor);
                    enemy.transform.position = transform.position + Vector3.up; //TODO: Randomize 
                    points -= tmpInfo[idx].cost;
                } else {
                    //If this Enemy is to expensive, remove it from the selection proccess
                    tmpInfo.RemoveAt(idx);
                }
            }
            coolDownTimer = 0f;
        }
    }

    private void Start() {
        pathfinder = FindObjectOfType<WorldPathfinder>();
        Assert.IsNotNull(pathfinder);
        worldManager = FindObjectOfType<WorldManager>();
        Assert.IsNotNull(worldManager);
        Assert.IsNotNull(node, "Initialize was not called");
        pathToReactor = pathfinder.FindPathFromTo(node, worldManager.reactorNode);
        coolDownTimer = 0;
    }
}
