using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameSceneDirector : MonoBehaviour
{
    // タイルマップ
    [SerializeField]
    GameObject grid;

    [SerializeField]
    Tilemap tilemapCollider;

    // マップ全体の座標
    public Vector2 TilemapStart;
    public Vector2 TilemapEnd;
    public Vector2 WorldStart;
    public Vector2 WorldEnd;

    public PlayerController Player;

    [SerializeField]
    Transform parentTextDamage;

    [SerializeField]
    GameObject prefabTextDamage;

    // タイマー
    [SerializeField]
    Text textTimer;
    public float GameTimer;
    public float OldSeconds;

    // 敵生成
    [SerializeField]
    EnemySpawnerController enemySpawner;

    // プレイヤー生成
    [SerializeField]
    Slider sliderXP;

    [SerializeField]
    Slider sliderHP;

    [SerializeField]
    Text textLv;

    // 経験値
    [SerializeField]
    List<GameObject> prefabXP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // プレイヤー作成
        int playerId = 0;
        Player = CharacterSettings.Instance.CreatePlayer(
            playerId,
            this,
            enemySpawner,
            textLv,
            sliderHP,
            sliderXP
        );

        // タイマーの初期設定
        OldSeconds = -1;
        enemySpawner.Init(this, tilemapCollider);

        // カメラの移動できる範囲
        foreach (Transform item in grid.GetComponentInChildren<Transform>())
        {
            // 開始位置
            if (TilemapStart.x > item.position.x)
            {
                TilemapStart.x = item.position.x;
            }
            if (TilemapStart.y > item.position.y)
            {
                TilemapStart.y = item.position.y;
            }
            // 終了位置
            if (TilemapEnd.x < item.position.x)
            {
                TilemapEnd.x = item.position.x;
            }
            if (TilemapEnd.y < item.position.y)
            {
                TilemapEnd.y = item.position.y;
            }
        }

        // 画面縦半分の描画範囲（default: 5 tiles）
        float cameraSize = Camera.main.orthographicSize;
        // 画面縦横比
        float aspect = (float)Screen.width / (float)Screen.height;
        // プレイヤーの移動可能範囲
        WorldStart = new Vector2(TilemapStart.x - cameraSize * aspect, TilemapStart.y - cameraSize);
        WorldEnd = new Vector2(TilemapEnd.x + cameraSize * aspect, TilemapEnd.y + cameraSize);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGameTimer();
    }

    // ダメージ表示
    public void DisplayDamage(GameObject target, float damage)
    {
        GameObject obj = Instantiate(prefabTextDamage, parentTextDamage);
        obj.GetComponent<TextDamageController>().Init(target, damage);
    }

    // ゲームタイマー
    void UpdateGameTimer()
    {
        GameTimer += Time.deltaTime;

        // 前回と秒数が同じなら処理をしない
        int seconds = (int)GameTimer % 60;
        if (seconds == OldSeconds)
        {
            return;
        }

        textTimer.text = Utils.GetTextTimer(GameTimer);
        OldSeconds = seconds;
    }

    public void CreateXP(EnemyController enemy)
    {
        float xp = UnityEngine.Random.Range(enemy.Stats.XP, enemy.Stats.MaxXP);
        if (0 > xp)
        {
            return;
        }

        // 5未満
        GameObject prefab = prefabXP[0];

        // 10以上
        if (10 <= xp)
        {
            prefab = prefabXP[1];
        }

        // 初期化
        GameObject obj = Instantiate(prefab, enemy.transform.position, Quaternion.identity);
        XPController ctrl = obj.GetComponent<XPController>();
        ctrl.Init(this, xp);
    }
}
