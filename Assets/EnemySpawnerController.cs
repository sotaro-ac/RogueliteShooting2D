using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum SpawnType
{
    Normal,
    Group,
}

[Serializable]
public class EnemySpawnData
{
    // 説明用
    public string Title;

    // 出現経過時間
    public int ElapsedMinutes;
    public int ElapsedSeconds;

    // 出現タイプ
    public SpawnType SpawnType;

    // 生成時間
    public float SpawnDuration;

    // 生成数
    public int SpawnCountMax;

    // 生成する敵ID
    public List<int> EnemyIds;
}

public class EnemySpawnerController : MonoBehaviour
{
    // 敵データ
    [SerializeField]
    List<EnemySpawnData> enemySpawnDatas;

    // 生成した敵
    List<EnemyController> enemies;

    // シーンディレクター
    GameSceneDirector sceneDirector;

    // 当たり判定のあるタイルマップ
    Tilemap tilemapCollider;

    // 現在の参照データ
    EnemySpawnData enemySpawnData;

    // 経過時間

    float oldSeconds;
    float spawnTimer;

    // 現在のデータ位置
    int spawnDataIndex;

    // 現在の出現位置

    const float SpawnRadius = 13;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
