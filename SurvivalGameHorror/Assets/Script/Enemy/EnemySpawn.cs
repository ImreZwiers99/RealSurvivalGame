using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject Enemy;

    public void EnemySpawnNight()
    {
        Enemy.SetActive(true);
    }

    public void EnemyDespawn()
    {
        Enemy.SetActive(false);
    }
}
