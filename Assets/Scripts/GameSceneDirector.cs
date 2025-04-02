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

    // レベルアップパネル
    [SerializeField]
    PanelLevelUpController panelLevelUp;

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
        panelLevelUp.Init(this);

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

    // ゲーム開始/停止
    void SetEnabled(bool enabled = true)
    {
        this.enabled = enabled;
        Time.timeScale = (enabled) ? 1 : 0;
        Player.SetEnabled(enabled);
    }

    // ゲーム再開
    public void PlayGame(BonusData bonusData = null)
    {
        // アイテム追加
        Player.AddBonusData(bonusData);
        // TODO: ステータス反映

        // ゲーム再開
        SetEnabled();
    }

    // レベルアップ時
    public void DispPanelLevelUp()
    {
        // 追加したアイテム
        List<WeaponSpawnerStats> items = new List<WeaponSpawnerStats>();

        // 生成数
        int randomCount = panelLevelUp.GetButtonCount();

        // 武器の数が足りない場合は減らす
        int listCount = Player.GetUsableWeaponIds().Count;

        // ボーナスをランダムで生成
        if (listCount < randomCount)
        {
            randomCount = listCount;
        }

        // ボーナスをランダムで生成
        // TODO: アルゴリズムの改善 O(n^2) -> O(1)
        for (int i = 0; i < randomCount; i++)
        {
            // 装備可能な武器からランダムで選出
            WeaponSpawnerStats randomItem = Player.GetRamdomSpawnerStats();

            // 該当データがない場合
            if (null == randomItem)
            {
                continue;
            }

            // 重複なしで規定数まで選出する
            WeaponSpawnerStats findItem = items.Find(item => item.Id == randomItem.Id);
            if (null == findItem)
            {
                items.Add(randomItem);
            }
            else
            {
                i--;
            }
        }

        // レベルアップパネルを表示
        panelLevelUp.DispPanel(items);
        // ゲーム停止
        SetEnabled(false);
    }
}
