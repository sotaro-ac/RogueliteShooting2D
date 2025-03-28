using UnityEngine;

public class SlashController : BaseWeapon
{
    // トリガーが衝突した時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        AttackEnemy(collision);
    }
}
