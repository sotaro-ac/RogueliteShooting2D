using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // 移動とAnmation
    Rigidbody2D rigidbody2d;
    Animator animator;

    // SerializedField していたものは Init へ移行
    GameSceneDirector sceneDirector;
    Slider sliderHP;
    Slider sliderXP;

    public CharacterStats Stats;

    // 攻撃のクールダウン
    float attackCoolDownTimer;
    float attackCoolDownTimerMax = 0.5f;

    // 最大レベル
    int maxLv = 999;

    // 必要XP
    List<int> levelRequirements;

    // 敵生成装置：敵がいる位置を知るために必要
    EnemySpawnerController enemySpawner;

    // 向き
    public Vector2 Forward;

    // レベルテキスト
    Text textLv;

    [SerializeField]
    Vector3 sliderHPPositinOffset = new(0, 50, 0); // HPスライダーをプレイヤーの頭上に移動する

    // 現在装備中の武器
    public List<BaseWeaponSpawner> WeaponSpawners;

    public Dictionary<ItemData, int> ItemDatas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // rigidbody2d = GetComponent<Rigidbody2D>();
        // animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
        MovePlayer();
        MoveCamera();
        MoveSliderHP();
    }

    // 初期化
    public void Init(
        GameSceneDirector sceneDirector,
        EnemySpawnerController enemySpawner,
        CharacterStats characterStats,
        Text textLv,
        Slider sliderHP,
        Slider sliderXP
    )
    {
        // 変数の初期化
        this.levelRequirements = new List<int>();
        this.WeaponSpawners = new List<BaseWeaponSpawner>();
        this.ItemDatas = new Dictionary<ItemData, int>();

        this.sceneDirector = sceneDirector;
        this.enemySpawner = enemySpawner;
        this.Stats = characterStats;
        this.textLv = textLv;
        this.sliderHP = sliderHP;
        this.sliderXP = sliderXP;

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // プレイヤーの向き
        Forward = Vector2.right;

        // 経験値の閾値リストを作成
        levelRequirements.Add(0);
        for (int i = 1; i <= maxLv; ++i)
        {
            // 1つ前の閾値
            int prevxp = levelRequirements[i - 1];

            // Lv41以降はレベル毎に16XPずつ増加
            int addxp = 16;

            // レベル2までレベルアップするのに5XP
            if (i == 1)
            {
                addxp = 5;
            }
            else if (20 >= i)
            {
                addxp = 10;
            }
            else if (40 >= i)
            {
                addxp = 13;
            }
            // 必要経験値
            levelRequirements.Add(prevxp + addxp);
        }

        // Lv2の必要経験値
        Stats.MaxXP = levelRequirements[1];

        // UI初期化
        SetTextLv();
        SetSliderHP();
        SetSliderXP();
        MoveSliderHP();

        // 武器データセット
        foreach (var item in Stats.DefaultWeaponIds)
        {
            AddWeaponSpawner(item);
        }
    }

    // Playerの移動に関する処理
    void MovePlayer()
    {
        // 移動方向
        Vector2 dir = Vector2.zero;

        // 再生するAnimation
        string trigger = "";

        // TODO: WASD で移動 (より柔軟な方法で実装し直す)
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            dir += Vector2.up;
            trigger = "isUp";
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            dir -= Vector2.up;
            trigger = "isDown";
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            dir += Vector2.right;
            trigger = "isRight";
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            dir -= Vector2.right;
            trigger = "isLeft";
        }

        // 入力がなければ処理を終える
        if (dir == Vector2.zero)
        {
            return;
        }

        // プレイヤー移動
        rigidbody2d.position += Stats.MoveSpeed * Time.deltaTime * dir.normalized;

        // アニメーションを再生
        animator.SetTrigger(trigger);

        // 移動範囲を制御
        // 始点
        if (rigidbody2d.position.x < sceneDirector.WorldStart.x)
        {
            Vector2 pos = rigidbody2d.position;
            pos.x = sceneDirector.WorldStart.x;
            rigidbody2d.position = pos;
        }
        if (rigidbody2d.position.y < sceneDirector.WorldStart.y)
        {
            Vector2 pos = rigidbody2d.position;
            pos.y = sceneDirector.WorldStart.y;
            rigidbody2d.position = pos;
        }
        // 終点
        if (sceneDirector.WorldEnd.x < rigidbody2d.position.x)
        {
            Vector2 pos = rigidbody2d.position;
            pos.x = sceneDirector.WorldEnd.x;
            rigidbody2d.position = pos;
        }
        if (sceneDirector.WorldEnd.y < rigidbody2d.position.y)
        {
            Vector2 pos = rigidbody2d.position;
            pos.y = sceneDirector.WorldEnd.y;
            rigidbody2d.position = pos;
        }

        // プレイヤーの向きを保持する
        Forward = dir;
    }

    // カメラの中心をプレイヤー位置に追従する
    void MoveCamera()
    {
        Vector3 pos = this.transform.position;
        pos.z = Camera.main.transform.position.z;

        // 始点
        if (pos.x < sceneDirector.TilemapStart.x)
        {
            pos.x = sceneDirector.TilemapStart.x;
        }
        if (pos.y < sceneDirector.TilemapStart.y)
        {
            pos.y = sceneDirector.TilemapStart.y;
        }
        if (sceneDirector.TilemapEnd.x < pos.x)
        {
            pos.x = sceneDirector.TilemapEnd.x;
        }
        if (sceneDirector.TilemapEnd.y < pos.y)
        {
            pos.y = sceneDirector.TilemapEnd.y;
        }

        // カメラの位置を更新する
        Camera.main.transform.position = pos;
    }

    // HPスライダーの位置をプレイヤーに追従
    void MoveSliderHP()
    {
        // ワールド座標をスクリーン座標に変換
        Vector3 pos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        sliderHP.transform.position = pos + sliderHPPositinOffset;
    }

    //
    public void Damage(float attack)
    {
        if (!enabled)
        {
            return;
        }

        float damage = Mathf.Max(0, attack - Stats.Defense);
        Stats.HP -= damage;

        // ダメージ表示
        sceneDirector.DisplayDamage(gameObject, damage);

        // ゲームオーバー
        if (0 > Stats.HP)
        {
            // 操作できないようにする
            SetEnabled(false);

            // アニメーション
            transform
                .DOScale(new Vector2(5, 0), 2)
                .SetUpdate(true)
                .OnComplete(() => sceneDirector.DispPanelGameOver());
        }

        if (0 > Stats.HP)
        {
            Stats.HP = 0;
        }

        SetSliderHP();
    }

    // HPスライダーの値を更新
    void SetSliderHP()
    {
        sliderHP.maxValue = Stats.MaxHP;
        sliderHP.value = Stats.HP;
    }

    // XPスライダーの値を更新
    void SetSliderXP()
    {
        sliderXP.maxValue = Stats.MaxXP;
        sliderXP.value = Stats.XP;
    }

    // 衝突したとき
    private void OnCollisionEnter2D(Collision2D collision)
    {
        AttackEnemy(collision);
    }

    // 衝突している間
    private void OnCollisionStay2D(Collision2D collision)
    {
        AttackEnemy(collision);
    }

    // 衝突が終わったとき
    private void OnCollisionExit2D(Collision2D collision)
    {
        // throw new System.NotImplementedException();
    }

    // 敵へ攻撃する
    void AttackEnemy(Collision2D collision)
    {
        // 敵以外
        if (!collision.gameObject.TryGetComponent<EnemyController>(out var enemy))
        {
            return;
        }
        // タイマー未消化
        if (0 < attackCoolDownTimer)
        {
            return;
        }

        enemy.Damage(Stats.Attack);
        attackCoolDownTimer = attackCoolDownTimerMax;
    }

    // 各種タイマー更新
    void UpdateTimer()
    {
        if (0 < attackCoolDownTimer)
        {
            attackCoolDownTimer -= Time.deltaTime;
        }
    }

    // レベルテキスト更新
    void SetTextLv()
    {
        // TODO: 最大レベルの桁数分の幅を持つ空白右詰めで表示
        // e.g. maxLv: 999, Stats.Lv: 12 -> " 12"
        // textLv.text = "Lv." + Stats.Lv.ToString().PadLeft(maxLv.ToString().Length);
        textLv.text = "Lv." + Stats.Lv;
    }

    void AddWeaponSpawner(int id)
    {
        // TODO: 装備済みならレベルアップする
        BaseWeaponSpawner spawner = WeaponSpawners.Find(item => item.Stats.Id == id);

        if (spawner)
        {
            spawner.LevelUp();
            return;
        }

        // 新規追加
        spawner = WeaponSpawnerSettings.Instance.CreateWeaponSpawner(id, enemySpawner, transform);

        if (null == spawner)
        {
            Debug.LogError("武器データがありません");
            return;
        }

        // 装備済みリストへ追加
        WeaponSpawners.Add(spawner);
    }

    // 経験値取得
    public void GetXP(float xp)
    {
        Stats.XP += xp;

        // レベル上限
        if (levelRequirements.Count - 1 < Stats.Lv)
        {
            return;
        }

        // レベルアップ
        if (levelRequirements[Stats.Lv] <= Stats.XP)
        {
            Stats.Lv++;

            // 次の経験値
            if (Stats.Lv < levelRequirements.Count)
            {
                Stats.XP = 0;
                Stats.MaxXP = levelRequirements[Stats.Lv];
            }

            // レベルアップパネルを表示
            sceneDirector.DispPanelLevelUp();

            SetTextLv();
        }

        // 表示更新
        SetSliderXP();
    }

    // 装備可能な武器リスト
    public List<int> GetUsableWeaponIds()
    {
        List<int> ret = new List<int>(Stats.UsableWeaponIds);

        // 装備可能数を超える場合は装備している武器のIDを返す
        if (Stats.UsableWeaponMax - 1 < WeaponSpawners.Count)
        {
            ret.Clear();
            foreach (var item in WeaponSpawners)
            {
                ret.Add(item.Stats.Id);
            }
        }

        return ret;
    }

    // 装備可能な武器をランダムで返す
    public WeaponSpawnerStats GetRamdomSpawnerStats()
    {
        // 装備可能な武器ID
        List<int> usableIds = GetUsableWeaponIds();

        // 装備可能な武器がない場合
        if (1 > usableIds.Count)
        {
            return null;
        }

        // 抽選する
        int rnd = UnityEngine.Random.Range(0, usableIds.Count);
        int id = usableIds[rnd];

        // 装備済みなら卯木のレベルのデータ
        BaseWeaponSpawner spawner = WeaponSpawners.Find(item => item.Stats.Id == id);
        if (spawner)
        {
            return spawner.GetLevelUpStats(true);
        }

        // 新規アイテムならレベル1のデータ
        return WeaponSpawnerSettings.Instance.Get(id, 1);
    }

    // アイテムを追加
    void AddItemData(int id)
    {
        ItemData itemData = ItemSettings.Instance.Get(id);

        if (null == itemData)
        {
            Debug.LogError("アイテムデータが見つかりません");
            return;
        }

        // データを追加
        Stats.AddItemData(itemData);

        // 取得済みリストへ追加
        ItemData key = null;
        foreach (var item in ItemDatas)
        {
            if (item.Key.Id == itemData.Id)
            {
                key = item.Key;
                break;
            }
        }

        if (null == key)
        {
            ItemDatas.Add(itemData, 0);
            key = itemData;
        }

        ItemDatas[key]++;
    }

    // レベルアップやアイテム取得時
    public void AddBonusData(BonusData bonusData)
    {
        if (null == bonusData)
        {
            return;
        }

        // 武器データ
        if (null != bonusData.WeaponSpawnerStats)
        {
            AddWeaponSpawner(bonusData.WeaponSpawnerStats.Id);
        }

        // アイテムデータ
        if (null != bonusData.ItemData)
        {
            AddItemData(bonusData.ItemData.Id);
        }

        // 表示更新
        SetSliderHP();
    }

    // アップデート停止
    public void SetEnabled(bool enabled = true)
    {
        this.enabled = enabled;

        // 武器
        foreach (var item in WeaponSpawners)
        {
            item.SetEnabled(enabled);
        }
    }
}
