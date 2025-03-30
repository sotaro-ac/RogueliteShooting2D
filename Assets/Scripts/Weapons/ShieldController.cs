using DG.Tweening;
using UnityEngine;

public class ShieldController : BaseWeapon
{
    // プレイヤーからの距離
    const float Radius = 1f;

    // 現愛の角度
    public float Angle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // フワッと表示する
        // * Modded: prefabの指定値を保持して、そのサイズまで膨張する
        Vector3 prevScale = transform.localScale;
        transform.localScale = new Vector3(0, 0, 0);
        // transform.DOScale(new Vector3(1, 1, 1), 1.5f).SetEase(Ease.OutBounce);
        transform.DOScale(prevScale, 1.5f).SetEase(Ease.OutBounce);
    }

    // Update is called once per frame
    void Update()
    {
        // 角度更新させながらプレイヤーの周囲を公転させる
        Angle -= stats.MoveSpeed * Time.deltaTime;

        // 半径 radius の円上のランダムな極座標
        float x = Mathf.Cos(Angle * Mathf.Deg2Rad) * Radius;
        float y = Mathf.Sin(Angle * Mathf.Deg2Rad) * Radius;

        // ポジション更新
        // transform.root: このMonoBehaviourオブジェクトの最上位階層（親）
        // 親がいない場合は自身が root と等価になる
        transform.position = transform.root.position + new Vector3(x, y, 0);
    }

    // トリガーが衝突したとき
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 敵以外の場合
        if (!collision.gameObject.TryGetComponent<EnemyController>(out var enemy))
        {
            return;
        }

        // 反対側へ跳ね返す
        Vector3 forward = enemy.transform.position - transform.root.position;
        enemy.GetComponent<Rigidbody2D>().AddForce(forward.normalized * 5);

        AttackEnemy(collision);
    }
}
