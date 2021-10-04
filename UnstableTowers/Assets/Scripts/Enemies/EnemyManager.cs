using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public Text waveMessage;
    private List<EnemySpawner> spawners = new List<EnemySpawner>();
    public List<Enemy> spawnables;
    private List<Enemy> enemies = new List<Enemy>();

    public Transform enemyParent;
    public Transform explosionParent;
    public bool instantFirstWave;
    public float timeBetweenWaves;
    public float timeMultiplier;
    public float minTime;
    public float pointsPerWave;
    public float pointMultiplier;
    public float maxPoints;

    public float spawnTimer = 0f;

    public void RegisterSpawner(EnemySpawner s) {
        s.enemyManager = this;
        spawners.Add(s);
    }

    public void RegisterEnemy(Enemy e) {
        enemies.Add(e);
    }

    public void DeRegisterEnemy(Enemy e) {
        enemies.Remove(e);
    }

    public Enemy GetEnemyInRange(Vector3 _pos, float _minRange, float _maxRange) {
        foreach(var e in enemies) {
            var m = (e.transform.position - _pos).magnitude;
            if (m > _minRange && m < _maxRange) {
                return e;
            }
        }
        return null;
    }

    public List<Enemy> GetEnemiesInRadius(Vector3 _pos, float radius) {
        List<Enemy> ret = new List<Enemy>();
        foreach (var e in enemies) {
            if ((e.transform.position - _pos).magnitude < radius) {
                ret.Add(e);
            }
        }
        return ret;
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
            spawner.SpawnEnemies(enemyParent, points, spawnables);
        }
    }

    void Start() {
        Assert.IsNotNull(enemyParent, "EnemyManager: no enemyParent is set");
        if (!instantFirstWave) {
            spawnTimer = timeBetweenWaves;
        } else {
            spawnTimer = 1f;
        }
    }

    void Update() {
        spawnTimer -= Time.deltaTime;
        waveMessage.text = "NEXT WAVE: " + spawnTimer.ToString("0.");
        if (spawnTimer <= 0f) {
            spawnEnemies();
            timeBetweenWaves *= timeMultiplier;
            if (timeBetweenWaves < minTime) {
                timeBetweenWaves = minTime;
            }
            pointsPerWave *= pointMultiplier;
            if (pointsPerWave > maxPoints) {
                pointsPerWave = maxPoints;
            }
            spawnTimer = timeBetweenWaves;
        }
    }
}
