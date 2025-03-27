using System;
using UnityEngine;

// [CreateAssetMenu(
//     fileName = "WeaponSpawnerSettings",
//     menuName = "Scriptable Objects/WeaponSpawnerSettings"
// )]
// public class WeaponSpawnerSettings : ScriptableObject { }


// 武器生成装置
[Serializable]
public class WeaponSpawnerStats : BaseStats
{
    // 生成装置のprefab
    public GameObject PrefabSpawner;

    // 武器のアイコン
    public Sprite Icon;

    // レベルアップ時に追加されるアイテムID
    public int LevelUpItemId;

    // 一度に生成する敵
    public float SpawnCount;

    // 生成タイマー
    public float SpawnTimerMin;
    public float SpawnTimerMax;

    // 生成時間取得
    public float GetRandomSpawnTimer()
    {
        return UnityEngine.Random.Range(SpawnTimerMin, SpawnTimerMax);
    }

    // TODO: アイテムを追加
}
