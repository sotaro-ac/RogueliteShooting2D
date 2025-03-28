using UnityEngine;

public class AxeController : BaseWeapon
{
    // Update is called once per frame
    void Update()
    {
        // 回転 (Z軸方向に回転を加える)
        transform.Rotate(new Vector3(0, 0, -1000 * Time.deltaTime));
    }

    // トリガーが衝突した時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        AttackEnemy(collision);
    }
}
