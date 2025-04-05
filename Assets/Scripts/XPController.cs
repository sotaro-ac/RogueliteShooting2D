using UnityEngine;

public class XPController : MonoBehaviour
{
    GameSceneDirector sceneDirector;
    new Rigidbody2D rigidbody2D;
    SpriteRenderer spriteRenderer;

    // 経験値
    float xp;

    // 一定時間経過で消える
    float aliveTimer = 50f;
    float fadeTime = 10f;

    // 初期化
    public void Init(GameSceneDirector sceneDirector, float xp)
    {
        this.sceneDirector = sceneDirector;
        this.xp = xp;

        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // ゲーム停止中
        if (!sceneDirector.enabled)
        {
            return;
        }

        // タイマー消化でフェードアウトする
        if (0 > aliveTimer)
        {
            // アルファ値を設定
            Color color = spriteRenderer.color;
            color.a -= 1.0f / fadeTime * Time.deltaTime;
            spriteRenderer.color = color;

            // 見えなくなったら消す
            if (0 >= color.a)
            {
                Destroy(gameObject);
                return;
            }
        }

        // プレイヤーとの距離
        float dist = Vector2.Distance(transform.position, sceneDirector.Player.transform.position);
        if (dist < sceneDirector.Player.Stats.PickUpRange)
        {
            float speed = sceneDirector.Player.Stats.MoveSpeed * 1.1f;
            Vector2 foward = sceneDirector.Player.transform.position - transform.position;
            rigidbody2D.position += foward.normalized * speed * Time.deltaTime;
        }
    }

    // トリガーが衝突した時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤー以外と接触
        if (!collision.gameObject.TryGetComponent<PlayerController>(out var player))
        {
            return;
        }

        // 経験値
        player.GetXP(xp);
        Destroy(gameObject);
    }
}
