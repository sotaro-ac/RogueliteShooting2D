using System;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(
//     fileName = "WeaponSpawnerSettings",
//     menuName = "Scriptable Objects/WeaponSpawnerSettings"
// )]
// public class WeaponSpawnerSettings : ScriptableObject { }

// 右クリックメニューに表示
// filename はデフォルトのファイル名
[CreateAssetMenu(
    fileName = "WeaponSpawnerSettings",
    menuName = "Scriptable Objects/WeaponSpawnerSettings"
)]
public class WeaponSpawnerSettings : ScriptableObject
{
    // データ
    public List<WeaponSpawnerStats> datas;
    static WeaponSpawnerSettings instance;
    public static WeaponSpawnerSettings Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<WeaponSpawnerSettings>(nameof(WeaponSpawnerSettings));
            }

            return instance;
        }
    }

    // リストのIDからデータを検索する
    public WeaponSpawnerStats Get(int id, int lv)
    {
        // 指定されたレベルのデータがなければ最も高いレベルのデータを返す
        WeaponSpawnerStats ret = null;

        foreach (var item in datas)
        {
            if (id != item.Id)
            {
                continue;
            }

            // 指定レベルと一致
            if (lv == item.Lv)
            {
                return (WeaponSpawnerStats)item.GetCopy();
            }

            // 仮のデータがセットされていないか調べ、超えるレベルであれば入れ替える
            if (null == ret)
            {
                ret = item;
            }
            // 探しているレベル未満であり、暫定データより大きい
            else if (item.Lv < lv && ret.Lv < item.Lv)
            {
                ret = item;
            }
        }

        return (WeaponSpawnerStats)datas.Find(item => item.Id == id).GetCopy();
    }

    // 作成
    public BaseWeaponSpawner CreateWeaponSpawner(
        int id,
        EnemySpawnerController enemySpawner,
        Transform parent = null
    )
    {
        // データ取得
        WeaponSpawnerStats stats = Instance.Get(id, 1);
        // オブジェクト作成
        GameObject obj = Instantiate(stats.PrefabSpawner, parent);
        // データをセット
        BaseWeaponSpawner spawner = obj.GetComponent<BaseWeaponSpawner>();
        spawner.Init(enemySpawner, stats);

        return spawner;
    }
}

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
