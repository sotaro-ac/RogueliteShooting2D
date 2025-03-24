using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // 移動とAnmation
    Rigidbody2D rigidbody2d;
    Animator animator;

    [SerializeField]
    float moveSpeed = 2.0f;

    // TODO: 後で Init へ移行
    [SerializeField]
    GameSceneDirector sceneDirector;

    [SerializeField]
    Slider sliderHP;

    [SerializeField]
    Slider sliderXP;

    public CharacterStats Stats;

    // 攻撃のクールダウン
    float attackCoolDownTimer;
    float attackCoolDownTimerMax = 0.5f;

    [SerializeField]
    Vector3 sliderHPPositinOffset = new(0, 50, 0); // HPスライダーをプレイヤーの頭上に移動する

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
        MovePlayer();
        MoveCamera();
        MoveSliderHP();
    }

    // Playerの移動に関する処理
    void MovePlayer()
    {
        // 移動方向
        Vector2 dir = Vector2.zero;

        // 再生するAnimation
        string trigger = "";

        if (Input.GetKey(KeyCode.UpArrow))
        {
            dir += Vector2.up;
            trigger = "isUp";
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            dir -= Vector2.up;
            trigger = "isDown";
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            dir += Vector2.right;
            trigger = "isRight";
        }
        if (Input.GetKey(KeyCode.LeftArrow))
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
        rigidbody2d.position += moveSpeed * Time.deltaTime * dir.normalized;

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

        // TODO: ゲームオーバー
        if (0 > Stats.HP) { }

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
}
