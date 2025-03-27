using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    // 親の生成装置
    protected BaseWeaponSpawner spawner;

    // 武器ステータス
    protected WeaponSpawnerStats stats;

    // 物理挙動
    protected Rigidbody2D rigidbody2d;

    // 方向
    protected Vector2 forward;

    // 初期化
    public void Init(BaseWeaponSpawner spawner, Vector2 forward)
    {
        // 親の生成装置
        this.spawner = spawner;
        // 武器データセット
        this.stats = (WeaponSpawnerStats)spawner.Stats.GetCopy();
        // 進む方向
        this.forward = forward;
        // 物理挙動
        this.rigidbody2d = GetComponent<Rigidbody2D>();

        // 生存時間があれば設定する
        if (-1 < stats.AliveTime)
        {
            Destroy(gameObject, stats.AliveTime);
        }
    }

    // 敵へ攻撃
    protected void AttackEnemy(Collider2D collider2d, float attack)
    {
        // 敵ではない場合
        if (!collider2d.gameObject.TryGetComponent<EnemyController>(out var enemy))
        {
            return;
        }
        // 攻撃
        float damage = enemy.Damage(attack);
        // 総ダメージを計算
        spawner.TotalDamage += damage;

        // HP設定があれば自身もダメージ
        if (0 > stats.HP)
        {
            return;
        }
        if (0 > --stats.HP)
        {
            Destroy(gameObject);
        }
    }

    // 敵へ攻撃（デフォルトの攻撃力）
    protected void AttackEnemy(Collider2D collider2d)
    {
        AttackEnemy(collider2d, stats.Attack);
    }
}
