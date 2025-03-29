using UnityEngine;

public class ArrowController : BaseWeapon
{
    // ターゲット
    public EnemyController Target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 進行方向 (ベクトル)
        Vector2 forward = Target.transform.position - transform.position;
        // 角度に変換する
        float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        // 角度を代入
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Update is called once per frame
    void Update()
    {
        // ターゲットがいない
        if (!Target)
        {
            Destroy(gameObject);
            return;
        }

        // 移動
        Vector2 forward = Target.transform.position - transform.position;
        rigidbody2d.position += forward.normalized * stats.MoveSpeed * Time.deltaTime;
    }

    // トリガーが衝突したとき
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 敵以外の場合
        if (!collision.gameObject.TryGetComponent<EnemyController>(out var enemy))
        {
            return;
        }

        // 通常ダメージ
        float attack = stats.Attack;

        // ターゲットと衝突
        if (Target == enemy)
        {
            // Target が null になることでUpdate時に矢が消滅する
            Target = null;
        }
        // ターゲット以外の敵に対しては1/3程度のダメージ
        else
        {
            attack /= 3;
        }

        AttackEnemy(collision, attack);
    }
}
