using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    // 宝箱関連
    [SerializeField]
    PanelTreasureChestController panelTreasureChest;

    [SerializeField]
    GameObject prefabTreasureChest;

    [SerializeField]
    List<int> treasureChestItemIds;

    [SerializeField]
    float treasureChestTimerMin;

    [SerializeField]
    float treasureChestTimerMax;
    float treasureChestTimer;

    // 左上に表示するアイコン
    [SerializeField]
    Transform canvas;

    [SerializeField]
    GameObject prefabImagePlayerIcon;
    Dictionary<BaseWeaponSpawner, GameObject> playerWeaponIcons;
    Dictionary<ItemData, GameObject> playerItemIcon;

    // * User Modified
    const float PlayerIconStartX = 2.5f;
    const float PlayerIconStartY = -22.5f;

    // 倒した敵のカウント
    [SerializeField]
    Text textDefeatedEnemy;
    public int DefeatedEnemyCount;

    // ゲームオーバー
    [SerializeField]
    PanelGameOverController panelGameOver;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 変数初期化
        playerWeaponIcons = new Dictionary<BaseWeaponSpawner, GameObject>();
        playerItemIcon = new Dictionary<ItemData, GameObject>();

        // プレイヤー作成
        int playerId = TitleSceneDirector.CharacterId;
        Player = CharacterSettings.Instance.CreatePlayer(
            playerId,
            this,
            enemySpawner,
            textLv,
            sliderHP,
            sliderXP
        );

        // 初期設定
        OldSeconds = -1;
        enemySpawner.Init(this, tilemapCollider);
        panelLevelUp.Init(this);
        panelTreasureChest.Init(this);
        panelGameOver.Init(this);

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

        // 初期値
        treasureChestTimer = Random.Range(treasureChestTimerMin, treasureChestTimerMax);
        DefeatedEnemyCount = -1;

        // アイコン更新
        DispPlayerIcon();

        // 倒した敵を更新
        AddDefeatedEnemy();

        // TimerScaleリセット
        SetEnabled();
    }

    // Update is called once per frame
    void Update()
    {
        // ゲームタイマー更新
        UpdateGameTimer();
        // ステータス反映
        DispPlayerIcon();
        // 宝箱生成
        UpdateTreasureChestSpawner();
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

    // 宝箱パネルを表示
    public void DispPanelTreasureChest()
    {
        // ランダムアイテム
        ItemData item = GetRandomItemData();
        // データなし
        if (null == item)
        {
            return;
        }

        // パネル表示
        panelTreasureChest.DispPanel(item);

        // ゲーム中断
        SetEnabled(false);
    }

    // アイテムをランダムで返す
    ItemData GetRandomItemData()
    {
        if (1 > treasureChestItemIds.Count)
        {
            return null;
        }

        // 抽選
        int rnd = Random.Range(0, treasureChestItemIds.Count);
        return ItemSettings.Instance.Get(treasureChestItemIds[rnd]);
    }

    // 宝箱生成
    void UpdateTreasureChestSpawner()
    {
        // タイマー
        treasureChestTimer -= Time.deltaTime;
        // タイマー未消化
        if (0 < treasureChestTimer)
        {
            return;
        }

        // 生成場所
        float x = Random.Range(WorldStart.x, WorldEnd.x);
        float y = Random.Range(WorldStart.y, WorldEnd.y);

        // 当たり判定のあるタイル上かどうか
        if (Utils.IsColliderTile(tilemapCollider, new Vector2(x, y)))
        {
            return;
        }

        GameObject obj = Instantiate(
            prefabTreasureChest,
            new Vector3(x, y, 0),
            Quaternion.identity
        );
        obj.GetComponent<TreasureChestController>().Init(this);

        // 次のタイマーをセット
        treasureChestTimer = Random.Range(treasureChestTimerMin, treasureChestTimerMax);
    }

    // プレイヤーアイコンをセット
    void SetPlayerIcon(GameObject obj, Vector2 pos, Sprite icon, int count)
    {
        // 画像
        Transform image = obj.transform.Find("ImageIcon");
        image.GetComponent<Image>().sprite = icon;

        // テキスト
        Transform text = obj.transform.Find("TextCount");
        text.GetComponent<TextMeshProUGUI>().text = "" + count;

        // 場所
        obj.GetComponent<RectTransform>().anchoredPosition = pos;
    }

    // アイコンの表示を更新
    void DispPlayerIcon()
    {
        // 武器アイコン表示位置
        float x = PlayerIconStartX;
        float y = PlayerIconStartY;
        float w = prefabImagePlayerIcon.GetComponent<RectTransform>().sizeDelta.x + 1;

        foreach (var item in Player.WeaponSpawners)
        {
            // 作成済みのデータがあれば取得する
            playerWeaponIcons.TryGetValue(item, out GameObject obj);

            // なければ作成する
            if (!obj)
            {
                obj = Instantiate(prefabImagePlayerIcon, canvas);
                playerWeaponIcons.Add(item, obj);
            }

            // アイコンセット
            SetPlayerIcon(obj, new(x, y), item.Stats.Icon, item.Stats.Lv);

            // 次の位置
            x += w;
        }

        // アイテムアイコン表示位置
        x = PlayerIconStartX;
        y = PlayerIconStartY - w;

        foreach (var item in Player.ItemDatas)
        {
            // 作成済みのデータがあれば取得する
            playerItemIcon.TryGetValue(item.Key, out GameObject obj);

            // なければ作成する
            if (!obj)
            {
                obj = Instantiate(prefabImagePlayerIcon, canvas);
                playerItemIcon.Add(item.Key, obj);
            }

            // アイコンセット
            SetPlayerIcon(obj, new(x, y), item.Key.Icon, item.Value);

            // 次の位置
            x += w;
        }
    }

    // 倒した敵をカウント
    public void AddDefeatedEnemy()
    {
        DefeatedEnemyCount++;
        textDefeatedEnemy.text = "" + DefeatedEnemyCount;
    }

    // タイトルへ
    public void LoadSceneTitle()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("TitleScene");
    }

    // ゲームオーバーパネルを表示
    public void DispPanelGameOver()
    {
        // パネル表示
        panelGameOver.DispPanel(Player.WeaponSpawners);

        // ゲーム中断
        SetEnabled(false);
    }
}
