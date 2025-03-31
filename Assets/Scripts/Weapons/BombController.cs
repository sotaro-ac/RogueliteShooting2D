using System.Collections.Generic;
using UnityEngine;

public class BombController : BaseWeapon
{
    // 状態
    enum State
    {
        Bomb,
        Explosion,
        DamageFloor,
        Destroy,
    }

    State state;

    // アニメーター
    Animator animator;

    // アニメーション毎のタイマー
    Dictionary<State, float> animationTimer;

    // ダメージフロア滞在時間
    float damageFloorCoolDownTime = 0.5f;

    // 敵毎のタイマー（ダメージフロア時）
    Dictionary<EnemyController, float> damageFloorTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 初期化
        animationTimer = new Dictionary<State, float>();
        damageFloorTimer = new Dictionary<EnemyController, float>();
        animator = GetComponent<Animator>();

        // 爆弾時
        animationTimer.Add(State.Bomb, Random.Range(0.5f, 1.5f));
        // 爆発時
        animationTimer.Add(State.Explosion, 0.66f);
        // ダメージフロア
        animationTimer.Add(State.DamageFloor, 30f);

        // 初期状態
        state = State.Bomb;
    }

    // Update is called once per frame
    void Update()
    {
        // タイマー消化で次の状態へ
        if (animationTimer.ContainsKey(state))
        {
            animationTimer[state] -= Time.deltaTime;
            if (0 > animationTimer[state])
            {
                ChangeState(++state);
            }
        }
    }

    // 爆弾の状態を変える
    void ChangeState(State next)
    {
        if (State.Explosion == next)
        {
            animator.SetTrigger("isExplosion");
            rigidbody2d.gravityScale = 0;
            rigidbody2d.linearVelocity = Vector2.zero;
        }
        // ダメージフロア
        else if (State.DamageFloor == next)
        {
            animator.SetTrigger("isDamageFloor");
            // 下側に入れる
            GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        else if (State.Destroy == next)
        {
            Destroy(gameObject);
        }

        // 現在の状態
        state = next;
    }

    // 衝突時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 敵以外
        if (!collision.gameObject.TryGetComponent<EnemyController>(out var enemy))
        {
            return;
        }

        // 爆弾
        if (State.Bomb == state)
        {
            AttackEnemy(collision);
            ChangeState(State.Explosion);
        }
        // 爆発中
        else if (State.Explosion == state)
        {
            AttackEnemy(collision);
        }
    }

    // 衝突している間
    private void OnTriggerStay2D(Collider2D collision)
    {
        // ダメージフロアではない
        if (State.DamageFloor != state)
        {
            return;
        }

        // 敵以外
        if (!collision.gameObject.TryGetComponent<EnemyController>(out var enemy))
        {
            return;
        }

        // ターゲットのタイマーをセット
        damageFloorTimer.TryAdd(enemy, damageFloorCoolDownTime);
        // 敵ごとにタイマーを消化
        damageFloorTimer[enemy] -= Time.deltaTime;

        // 一定時間でダメージ
        if (0 > damageFloorTimer[enemy])
        {
            AttackEnemy(collision, stats.Attack / 3);
            damageFloorTimer[enemy] = damageFloorCoolDownTime;
        }
    }
}
