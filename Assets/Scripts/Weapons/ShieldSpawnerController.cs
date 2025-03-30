using UnityEngine;

public class ShieldSpawnerController : BaseWeaponSpawner
{
    // Update is called once per frame
    void Update()
    {
        // TODO: 一定時間で消えた箇所のシールドが復活するようにする

        // オブジェクトのカウント
        weapons.RemoveAll(item => item == null);

        // １つでも残っていたら終了
        if (0 < weapons.Count)
        {
            return;
        }

        // 全部なくなったらタイマー消化
        if (isSpawnTimerNotElapsed())
        {
            return;
        }

        // 武器生成
        for (int i = 0; i < Stats.SpawnCount; i++)
        {
            ShieldController ctrl = (ShieldController)CreateWeapon(transform.position, transform);

            // 初期角度
            ctrl.Angle = 360f / Stats.SpawnCount * i;
        }

        // 次のタイマー
        spawnTimer = Stats.GetRandomSpawnTimer();
    }
}
