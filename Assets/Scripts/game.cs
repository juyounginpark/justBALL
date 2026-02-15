using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game : MonoBehaviour
{
    public GameObject spikePrefab;
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 2f;
    public float minX = -8f;
    public float maxX = 8f;
    
    // 가시 이동 관련 설정
    public float startY = -5.69f;
    public float targetY = -3.42f;
    public float riseSpeed = 2f; 
    public float destroyTime = 5f;

    private List<Transform> activeSpikes = new List<Transform>();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        // 활성화된 모든 가시를 이동시킴
        for (int i = activeSpikes.Count - 1; i >= 0; i--)
        {
            Transform spike = activeSpikes[i];
            
            // 가시가 파괴되었으면 리스트에서 제거
            if (spike == null) 
            {
                activeSpikes.RemoveAt(i);
                continue;
            }

            // 목표 높이보다 낮으면 위로 이동
            if (spike.position.y < targetY)
            {
                // 부드럽게 이동 (MoveTowards)
                float newY = Mathf.MoveTowards(spike.position.y, targetY, riseSpeed * Time.deltaTime);
                spike.position = new Vector3(spike.position.x, newY, spike.position.z);
            }
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(waitTime);

            SpawnSpike();
        }
    }

    void SpawnSpike()
    {
        if (spikePrefab == null)
        {
             // Debug.LogWarning("Spike Prefab is not assigned!");
             return;
        }

        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, startY, 0f);

        GameObject newSpike = Instantiate(spikePrefab, spawnPosition, Quaternion.identity);
        activeSpikes.Add(newSpike.transform);
        
        // 가시(Spike)와 땅(Ground) 간의 물리적 충돌 무시
        Collider2D spikeCollider = newSpike.GetComponent<Collider2D>();
        if (spikeCollider != null)
        {
            GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
            foreach (GameObject ground in grounds)
            {
                Collider2D groundCollider = ground.GetComponent<Collider2D>();
                if (groundCollider != null)
                {
                    Physics2D.IgnoreCollision(spikeCollider, groundCollider);
                }
            }
        }

        Destroy(newSpike, destroyTime);
    }
}
