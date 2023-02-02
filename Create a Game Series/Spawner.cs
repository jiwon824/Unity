using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;

    LivingEntity playerEntity; // OnDeath �̺�Ʈ�� ����� ������ ��
    Transform playerT; // �÷��̾��� ��ġ�� ��� ����

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn; //�����ؾ� �� ���� �󸶳� ���Ҵ���
    int enemiesRemainingAlive; // ��� �ִ� ���� ��
    float nextSpawnTime; // ���� �ð�

    // MapGenerator�ʻ�����κ���, ����Ÿ�ϵ鿡 ���� ���۷����� �������� ����
    MapGenerator map;

    float timeBetweenCampingChecks = 2; // �󸶳� ���� üũ���� ������ ķ�� �˻� ����
    float nextCampCheckTime; // ���� �˻� ���� �ð�
    float campThresholdDistance = 1.5f; // ķ���� �ɷ� ���ֵ��� �ʱ� ���ؼ� �÷��̾ ķ�� üũ ���̿� �������� �� �ּ� �Ѱ� �Ÿ�
    Vector3 campPositionOld; // ���� �ֱٿ� ķ�� üũ�� �� �� �÷��̾ �־��� ���
    bool isCamping;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = Time.time + timeBetweenCampingChecks;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath; // playerEntity.OnDeath�� OnPlayerDeath �� �������ѳ��´�.

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;
                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }
        
            // �����ؾ� �� �� enemiesRemaningToSpawn�� 0���� ũ�� &&
            // ���� �ð��� ������ ���� �ð� nextSpawnTime ���� ũ�ٸ�
            if (enemiesRemainingToSpawn>0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine(SpawnEnemy());
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1; // ���� ��ȯ�Ǳ� ��, �� �� ���� ��¦ �Ÿ���
        float tileFlashSpeed = 4; // �ʴ� �� �� ��¦ �Ÿ���

        Transform spawnTile = map.GetRandomOpenTile(); // ���� ��ȯ�� ���� Ÿ��
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }

        //���� ��ȯ�� Ÿ���� ������ ������ ���� ���� ����Ÿ���� material�� ���� �ؾ� ��
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        // ���� ���� ��¦�Ÿ� �� �� ������ �ʿ�.
        Color initialColour = tileMat.color;
        Color fleshColour = Color.red;
        float spawnTimer = 0; // ��ȯ �ð�������

        //���� ���� ��¦�� ���� ������ ����
        while (spawnTimer < spawnDelay)
        {
            // ��¦�Ÿ��� �ӵ��� ���� ���������� tileFlashSpeed�� �����ְ�
            // 0�� 1�� �Դٰ��� �ؾ� �ϹǷ� 1�� �־���
            tileMat.color = Color.Lerp(initialColour, fleshColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null; // �� ������ ��ŭ ���
        }

        // �� ��ȯ
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    // �÷��̾ �׾��� �� ����� ��Ȱ��ȭ
    void OnPlayerDeath()
    {
        isDisabled = true;
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
    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }
    
    void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1]; // �迭�� 0���� �����ϱ� ������ currentWaveNumber - 1

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null) // null�ε� ȣ���ϸ� ������
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }
    
    [System.Serializable]
    public class Wave
    {
        public int enemyCount; // �ش� ���̺��� ���� ��
        public float timeBetweenSpawns; // �ش� ���̺��� ���� ����
    }
}
