using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawnerController : BaseWeaponSpawner
{
    // Update is called once per frame
    void Update()
    {
        if (isSpawnTimerNotElapsed())
        {
            return;
        }

        // 次のタイマー
        spawnTimer = Stats.GetRandomSpawnTimer();

        // 敵がいない場合
        if (1 > enemySpawner.GetEnemies().Count)
        {
            return;
        }

        for (int i = 0; i < (int)Stats.SpawnCount; i++)
        {
            // 武器生成
            ArrowController ctrl = (ArrowController)CreateWeapon(transform.position);

            // ランダムでターゲットを決定
            List<EnemyController> enemies = enemySpawner.GetEnemies();
            int rnd = Random.Range(0, enemies.Count);
            EnemyController target = enemies[rnd];

            ctrl.Target = target;
        }
    }
}
