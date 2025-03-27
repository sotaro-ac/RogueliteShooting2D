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

    // インスタンス読み込み時に呼び出される
    void Awake()
    {
        DG.Tweening.DOTween.SetTweensCapacity(tweenersCapacity: 500, sequencesCapacity: 200);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        // 敵生成データ更新
        UpdateEnemySpawnData();
        // 生成
        SpawnEnemy();
    }

    public void Init(GameSceneDirector sceneDirector, Tilemap tilemapCollider)
    {
        this.sceneDirector = sceneDirector;
        this.tilemapCollider = tilemapCollider;

        // 生成したい敵を保存
        enemies = new List<EnemyController>();
        spawnDataIndex = -1;
    }

    void SpawnEnemy()
    {
        // 現在のデータ
        if (null == enemySpawnData)
        {
            return;
        }

        // タイマー
        spawnTimer -= Time.deltaTime;
        if (0 < spawnTimer)
        {
            return;
        }

        if (SpawnType.Normal == enemySpawnData.SpawnType)
        {
            SpawnNormal();
        }
        else if (SpawnType.Group == enemySpawnData.SpawnType)
        {
            SpawnGroup();
        }

        spawnTimer = enemySpawnData.SpawnDuration;
    }

    // 通常の生成
    void SpawnNormal()
    {
        // プレイヤー位置
        Vector3 center = sceneDirector.Player.transform.position;

        // 敵生成
        for (int i = 0; i < enemySpawnData.SpawnCountMax; ++i)
        {
            // プレイヤーの周りから出現させる
            float angle = 360 / enemySpawnData.SpawnCountMax * i;
            // 半径 radius の円上のランダムな極座標
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * SpawnRadius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * SpawnRadius;

            Vector2 pos = center + new Vector3(x, y, 0);

            // 当たり判定のあるタイル上なら生成しない
            if (Utils.IsColliderTile(tilemapCollider, pos))
            {
                continue;
            }

            // 生成
            CreateRandomEnemy(pos);
        }
    }

    // ランダムなIDの敵を生成
    private void CreateRandomEnemy(Vector3 pos)
    {
        // データからランダムなIDを取得
        int rnd = UnityEngine.Random.Range(0, enemySpawnData.EnemyIds.Count);
        int id = enemySpawnData.EnemyIds[rnd];

        // 敵を生成
        EnemyController enemy = CharacterSettings.Instance.CreateEnemy(id, sceneDirector, pos);
        enemies.Add(enemy);
    }

    void SpawnGroup()
    {
        // プレイヤー位置
        Vector3 center = sceneDirector.Player.transform.position;

        // プレイヤーの周りから出現させる
        float angle = UnityEngine.Random.Range(0, 360);
        // 半径 radius の円上のランダムな極座標
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * SpawnRadius;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * SpawnRadius;

        // 生成位置
        center += new Vector3(x, y, 0);
        float radius = 0.5f;

        // 敵生成
        for (int i = 0; i < enemySpawnData.SpawnCountMax; ++i)
        {
            // プレイヤーの周りから出現させる
            angle = 360 / enemySpawnData.SpawnCountMax * i;
            // 半径 radius の円上のランダムな極座標
            x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            Vector2 pos = center + new Vector3(x, y, 0);

            // 当たり判定のあるタイル上なら生成しない
            if (Utils.IsColliderTile(tilemapCollider, pos))
            {
                continue;
            }

            // 生成
            CreateRandomEnemy(pos);
        }
    }

    // 経過秒数で敵生成データを入れ替える
    void UpdateEnemySpawnData()
    {
        // 経過秒数に違いがある場合
        if (oldSeconds == sceneDirector.OldSeconds)
        {
            return;
        }

        // 1つ先のデータを参照
        int idx = spawnDataIndex + 1;

        // データの最後
        if (enemySpawnDatas.Count - 1 < idx)
        {
            return;
        }

        // 設定された経過時間を超えていたらデータを入れ替える
        EnemySpawnData data = enemySpawnDatas[idx];
        int elapsedSeconds = data.ElapsedMinutes * 60 + data.ElapsedSeconds;

        if (elapsedSeconds < sceneDirector.GameTimer)
        {
            enemySpawnData = enemySpawnDatas[idx];

            // 次回用の設定
            spawnDataIndex = idx;
            spawnTimer = 0;
            oldSeconds = sceneDirector.OldSeconds;
        }
    }

    // 全ての敵を返す
    public List<EnemyController> GetEnemies()
    {
        enemies.RemoveAll(item => !item);
        return enemies;
    }
}
