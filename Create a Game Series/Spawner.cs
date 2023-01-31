using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn; //�����ؾ� �� ���� �󸶳� ���Ҵ���
    int enemiesRemainingAlive; // ��� �ִ� ���� ��
    float nextSpawnTime; // ���� �ð�

    void Start()
    {
        NextWave();
    }

    void Update()
    {
        // �����ؾ� �� �� enemiesRemaningToSpawn�� 0���� ũ�� &&
        // ���� �ð��� ������ ���� �ð� nextSpawnTime ���� ũ�ٸ�
        if (enemiesRemainingToSpawn>0 && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            // �� ��ȯ
            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;

        // ����ִ� ���� ���ٸ� NextWave ȣ��
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
            currentWave = waves[currentWaveNumber - 1]; // �迭�� 0���� �����ϱ� ������ currentWaveNumber - 1

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
        }
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount; // �ش� ���̺��� ���� ��
        public float timeBetweenSpawns; // �ش� ���̺��� ���� ����
    }
}
