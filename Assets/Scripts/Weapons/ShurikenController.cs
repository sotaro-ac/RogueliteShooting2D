using UnityEngine;

public class ShurikenController : BaseWeapon
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        // 回転
        transform.Rotate(new Vector3(0, 0, 1000 * Time.deltaTime));
        // 移動
        rigidbody2d.position += forward * stats.MoveSpeed * Time.deltaTime;
    }

    // トリガーが衝突した瞬間
    private void OnTriggerEnter2D(Collider2D collision)
    {
        AttackEnemy(collision);
    }
}
