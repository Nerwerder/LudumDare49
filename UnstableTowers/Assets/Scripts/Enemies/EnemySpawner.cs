using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public void spawnEnemies(Transform parent, float points, List<EnemyManager.EnemyInfo> info) {
        var enemy = Instantiate(info[0].prefab, parent);
        enemy.transform.position = transform.position + Vector3.up;
    }
}
