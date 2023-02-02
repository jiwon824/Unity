using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;

    LivingEntity playerEntity; // OnDeath 이벤트를 사용해 구현할 거
    Transform playerT; // 플레이어의 위치를 계속 추적

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn; //스폰해야 할 적이 얼마나 남았는지
    int enemiesRemainingAlive; // 살아 있는 적의 수
    float nextSpawnTime; // 스폰 시간

    // MapGenerator맵생성기로부터, 오픈타일들에 대한 레퍼런스를 가져오고 싶음
    MapGenerator map;

    float timeBetweenCampingChecks = 2; // 얼마나 자주 체크할지 설정할 캠핑 검사 간격
    float nextCampCheckTime; // 다음 검사 예정 시간
    float campThresholdDistance = 1.5f; // 캠핑한 걸로 간주되지 않기 위해서 플레이어가 캠핑 체크 사이에 움직여야 할 최소 한계 거리
    Vector3 campPositionOld; // 가장 최근에 캠핑 체크를 할 때 플레이어가 있었던 장소
    bool isCamping;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = Time.time + timeBetweenCampingChecks;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath; // playerEntity.OnDeath에 OnPlayerDeath 를 구독시켜놓는다.

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
        
            // 스폰해야 할 적 enemiesRemaningToSpawn이 0보다 크고 &&
            // 현재 시간이 다음번 스폰 시간 nextSpawnTime 보다 크다면
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
        float spawnDelay = 1; // 적이 소환되기 전, 몇 초 동안 반짝 거릴지
        float tileFlashSpeed = 4; // 초당 몇 번 반짝 거릴지

        Transform spawnTile = map.GetRandomOpenTile(); // 적을 소환할 랜덤 타일
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }

        //적이 소환될 타일을 빨갛게 빛내기 위해 빛날 랜덤타일의 material에 접근 해야 함
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        // 원래 색과 반짝거릴 색 두 가지가 필요.
        Color initialColour = tileMat.color;
        Color fleshColour = Color.red;
        float spawnTimer = 0; // 소환 시간측정기

        //원래 색과 반짝이 색을 번갈아 적용
        while (spawnTimer < spawnDelay)
        {
            // 반짝거리는 속도가 점점 빨라지도록 tileFlashSpeed를 곱해주고
            // 0과 1을 왔다갔다 해야 하므로 1을 넣어줌
            tileMat.color = Color.Lerp(initialColour, fleshColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null; // 한 프레임 만큼 대기
        }

        // 적 소환
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    // 플레이어가 죽었을 때 기능을 비활성화
    void OnPlayerDeath()
    {
        isDisabled = true;
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
    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }
    
    void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1]; // 배열은 0부터 시작하기 때문에 currentWaveNumber - 1

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null) // null인데 호출하면 에러남
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }
    
    [System.Serializable]
    public class Wave
    {
        public int enemyCount; // 해당 웨이브의 적의 수
        public float timeBetweenSpawns; // 해당 웨이브의 스폰 간격
    }
}
