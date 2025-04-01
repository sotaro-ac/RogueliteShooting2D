using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 右クリックメニューに表示
// filename はデフォルトのファイル名
[CreateAssetMenu(fileName = "CharacterSettings", menuName = "Scriptable Objects/CharacterSettings")]
public class CharacterSettings : ScriptableObject
{
    // キャラクターデータ
    public List<CharacterStats> datas;
    static CharacterSettings instance;
    public static CharacterSettings Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<CharacterSettings>(nameof(CharacterSettings));
            }

            return instance;
        }
    }

    // リストのIDからデータを検索する
    public CharacterStats Get(int id)
    {
        return (CharacterStats)datas.Find(item => item.Id == id).GetCopy();
    }

    public EnemyController CreateEnemy(int id, GameSceneDirector sceneDirector, Vector3 position)
    {
        // ステータス取得
        CharacterStats stats = Instance.Get(id);
        // オブジェクト
        GameObject obj = Instantiate(stats.Prefab, position, Quaternion.identity);

        // データセット
        EnemyController ctrl = obj.GetComponent<EnemyController>();
        ctrl.Init(sceneDirector, stats);

        return ctrl;
    }

    // プレイヤー生成
    public PlayerController CreatePlayer(
        int id,
        GameSceneDirector sceneDirector,
        EnemySpawnerController enemySpawner,
        Text textLv,
        Slider sliderHP,
        Slider sliderXP
    )
    {
        // ステータス取得
        CharacterStats stats = instance.Get(id);
        // オブジェクト作成
        GameObject obj = Instantiate(stats.Prefab, Vector3.zero, Quaternion.identity);

        // データセット
        PlayerController ctrl = obj.GetComponent<PlayerController>();
        ctrl.Init(sceneDirector, enemySpawner, stats, textLv, sliderHP, sliderXP);

        return ctrl;
    }
}

// 敵の動き
public enum MoveType
{
    // キャラクターに向かって進む
    TargetPlayer,

    // 一方向に進む
    TargetDirection,
}

[Serializable]
public class CharacterStats : BaseStats
{
    // キャラクターのprefab
    public GameObject Prefab;

    // 初期装備武器ID
    public List<int> DefaultWeaponIds;

    // 装備可能武器ID
    public List<int> UsableWeaponIds;

    // 装備可能数
    public int UsableWeaponMax;

    // 移動タイプ
    public MoveType MoveType;

    // アイテム追加
    public void AddItemData(ItemData itemData)
    {
        foreach (var item in itemData.Bonuses)
        {
            AddBonus(item);
        }
    }
}
