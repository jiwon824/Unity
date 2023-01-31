using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn; //스폰해야 할 적이 얼마나 남았는지
    int enemiesRemainingAlive; // 살아 있는 적의 수
    float nextSpawnTime; // 스폰 시간

    void Start()
    {
        NextWave();
    }

    void Update()
    {
        // 스폰해야 할 적 enemiesRemaningToSpawn이 0보다 크고 &&
        // 현재 시간이 다음번 스폰 시간 nextSpawnTime 보다 크다면
        if (enemiesRemainingToSpawn>0 && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            // 적 소환
            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;

        // 살아있는 적이 없다면 NextWave 호출
        if (enemiesRemainingAlive == 0 && enemiesRemainingToSpawn == 0)
        {
            NextWave();
        }
    }

    void NextWave()
    {
        currentWaveNumber++;
        print("Waves: " + currentWaveNumber);
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1]; // 배열은 0부터 시작하기 때문에 currentWaveNumber - 1

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
        }
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount; // 해당 웨이브의 적의 수
        public float timeBetweenSpawns; // 해당 웨이브의 스폰 간격
    }
}
