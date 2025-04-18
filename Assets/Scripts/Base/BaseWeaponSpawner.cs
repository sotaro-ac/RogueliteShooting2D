using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponSpawner : MonoBehaviour
{
    // 武器のprefab
    [SerializeField]
    GameObject PrefabWeapon;

    // 武器データ
    public WeaponSpawnerStats Stats;

    // 与えた総ダメージ
    public float TotalDamage;

    // 稼働タイマー
    public float TotalTimer;

    // 生成タイマー
    protected float spawnTimer;

    // 生成した武器のリスト
    public List<BaseWeapon> weapons;

    // 敵生成装置
    protected EnemySpawnerController enemySpawner;

    // 初期化
    public void Init(EnemySpawnerController enemySpawner, WeaponSpawnerStats stats)
    {
        // 変数初期化
        weapons = new List<BaseWeapon>();
        this.enemySpawner = enemySpawner;
        this.Stats = stats;
    }

    // 稼働タイマー
    private void FixedUpdate()
    {
        TotalTimer += Time.fixedDeltaTime;
    }

    // 武器生成
    protected BaseWeapon CreateWeapon(Vector3 position, Vector2 forward, Transform parent = null)
    {
        // 生成
        GameObject obj = Instantiate(
            PrefabWeapon,
            position,
            PrefabWeapon.transform.rotation,
            parent
        );
        // 共通データセット
        BaseWeapon weapon = obj.GetComponent<BaseWeapon>();
        // データ初期化
        weapon.Init(this, forward);
        // 武器リストへ追加
        weapons.Add(weapon);

        return weapon;
    }

    // 武器生成（簡略化版）
    protected BaseWeapon CreateWeapon(Vector3 position, Transform parent = null)
    {
        return CreateWeapon(position, Vector2.zero, parent);
    }

    // 武器のUpdateを停止する
    public void SetEnabled(bool enabled = true)
    {
        this.enabled = enabled;
        // オブジェクトを削除
        weapons.RemoveAll(item => !item);
        // 生成した武器を停止
        foreach (var item in weapons)
        {
            item.enabled = enabled;
            // Rigidbodyを停止
            item.GetComponent<Rigidbody2D>().simulated = enabled;
        }
    }

    // タイマー消化チェック
    protected bool isSpawnTimerNotElapsed()
    {
        // タイマーを消化
        spawnTimer -= Time.deltaTime;
        if (0 < spawnTimer)
        {
            return true;
        }

        return false;
    }

    // レベルアップ時のデータを返す
    public WeaponSpawnerStats GetLevelUpStats(bool isNextLevel = false)
    {
        // 次のレベル
        int nextLv = Stats.Lv + 1;

        WeaponSpawnerStats ret = WeaponSpawnerSettings.Instance.Get(Stats.Id, nextLv);

        // 上書きデータあり
        if (Stats.Lv < ret.Lv)
        {
            // pass
        }
        else
        {
            // 説明をアイテムのものに書き換える
            ItemData itemData = ItemSettings.Instance.Get(Stats.LevelUpItemId);
            ret.Description = itemData.Description;
        }

        // レベルを1上げて返すかどうか
        if (isNextLevel)
        {
            ret.Lv = nextLv;
        }

        return ret;
    }

    // レベルアップ
    public void LevelUp()
    {
        // 現在のレベル
        int lv = Stats.Lv;

        // 次のレベルのデータ
        WeaponSpawnerStats nextData = GetLevelUpStats();

        // 現在のレベルと異なれば上書き
        if (Stats.Lv < nextData.Lv)
        {
            Stats = nextData;
        }
        // 装備していなければアイテムデータを追加
        else
        {
            // 説明をアイテムのものに書き換える
            ItemData itemData = ItemSettings.Instance.Get(Stats.LevelUpItemId);
            Stats.AddItemData(itemData);
        }

        Stats.Lv = lv + 1;
    }
}
