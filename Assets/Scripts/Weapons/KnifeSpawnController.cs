using UnityEngine;

public class KnifeSpawnController : BaseWeaponSpawner
{
    // 一度の生成に時差をつける
    int onceSpawnCount;
    float onceSpawnTime = 0.1f;
    PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onceSpawnCount = (int)Stats.SpawnCount;
        player = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpawnTimerNotElapsed())
        {
            return;
        }

        // 武器生成
        KnifeController ctrl = (KnifeController)CreateWeapon(transform.position, player.Forward);

        // 次の生成タイマー
        spawnTimer = onceSpawnTime;
        onceSpawnCount--;

        // 1回の生成が終わったらリセット
        if (1 > onceSpawnCount)
        {
            spawnTimer = Stats.GetRandomSpawnTimer();
            onceSpawnCount = (int)Stats.SpawnCount;
        }
    }
}
