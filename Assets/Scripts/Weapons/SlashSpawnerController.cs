using UnityEngine;

public class SlashSpawnerController : BaseWeaponSpawner
{
    // 一度の生成に時差をつける
    int onceSpawnCount;
    float onceSpawnTime = 0.3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 固有のパラメータを設定
        onceSpawnCount = (int)Stats.SpawnCount;
    }

    // Update is called once per frame
    void Update()
    {
        // タイマーを消化
        if (isSpawnTimerNotElapsed())
        {
            return;
        }

        // 偶数で左右に出す
        int dir = (onceSpawnCount % 2 == 0) ? 1 : -1;

        // 場所
        Vector3 pos = transform.position;
        pos.x += 2f * dir;

        // 生成
        SlashController ctrl = (SlashController)CreateWeapon(pos, transform);

        SoundController.Instance.PlaySE(1);

        // 左右で角度を変える
        ctrl.transform.eulerAngles = ctrl.transform.eulerAngles * dir;

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
