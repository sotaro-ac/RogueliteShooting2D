using UnityEngine;

public class PinwheelSpawnerController : BaseWeaponSpawner
{
    // Update is called once per frame
    void Update()
    {
        if (isSpawnTimerNotElapsed())
        {
            return;
        }

        //    武器を生成
        for (int i = 0; i < Stats.SpawnCount; i++)
        {
            CreateWeapon(transform.position);
        }

        spawnTimer = Stats.GetRandomSpawnTimer();
    }
}
