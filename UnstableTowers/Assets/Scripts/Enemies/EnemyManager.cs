using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyInfo
    {
        public GameObject prefab;
        public int cost;
    }

    private List<EnemySpawner> spawners = new List<EnemySpawner>();
    public List<EnemyInfo> enemies;

    public GameObject enemyParent;
    public bool instantFirstWave;
    public float timeBetweenWaves;
    public float timeMultiplier;
    public float pointsPerWave;
    public float pointMultiplier;

    private float spawnTimer = 0f;

    public void registerSpawner(EnemySpawner s) {
        spawners.Add(s);
    }

    public void spawnEnemies() {
        //How many Spawners will work?
        var activeSpawnerCount = Random.Range(0, (spawners.Count + 1));
        //Get (Random) Spawners
        List<EnemySpawner> tmpSpawners = new List<EnemySpawner>(spawners);
        List<EnemySpawner> activeSpawners = new List<EnemySpawner>();
        for (int k = 0; k < activeSpawnerCount; ++k) {
            var spawner = tmpSpawners[Random.Range(0, tmpSpawners.Count)];
            activeSpawners.Add(spawner);
            tmpSpawners.Remove(spawner);
        }
        //Split the Points and provide give the Order to spawn
        float tmpPoints = pointsPerWave;
        foreach (var spawner in activeSpawners) {
            float points = 0f;
            //The last of the spawner gets all the remaining Points
            if (spawner == activeSpawners[activeSpawners.Count - 1]) {
                points = tmpPoints;
            } else {
                points = Random.Range(0f, tmpPoints);
                tmpPoints -= points;
            }
            spawner.spawnEnemies(enemyParent.transform, points, enemies);
        }
    }

    void Start() {
        Assert.IsNotNull(enemyParent, "EnemyManager: no enemyParent is set");
        if (!instantFirstWave) {
            spawnTimer = timeBetweenWaves;
        }
    }

    void Update() {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f) {
            spawnEnemies();
            timeBetweenWaves *= timeMultiplier;
            pointsPerWave *= pointMultiplier;
            spawnTimer = timeBetweenWaves;
        }
    }
}
