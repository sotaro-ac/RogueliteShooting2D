using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public CharacterStats Stats;

    GameSceneDirector sceneDirector;
    Rigidbody2D rigidbody2d;

    // 攻撃のクールダウン
    float attackCoolDownTimer;
    float attackCoolDownTimerMax = 0.5f;

    // 向き
    Vector2 forward;

    enum State
    {
        Alive,
        Dead,
    }

    State state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        updateTimer();
        moveEnemy();
    }

    public void Init(GameSceneDirector sceneDirector, CharacterStats characterStats)
    {
        this.sceneDirector = sceneDirector;
        this.Stats = characterStats;

        rigidbody2d = GetComponent<Rigidbody2D>();

        // アニメーション
        // ランダムで緩急をつける
        float random = Random.Range(0.8f, 1.2f);
        float speed = 1 / Stats.MoveSpeed * random;

        float addx = 0.8f;
        float x = addx * random;
        transform.DOScale(x, speed).SetRelative().SetLoops(-1, LoopType.Yoyo);

        // 回転
        float addz = 10f;
        float z = Random.Range(-addz, addz) * random;
        Vector3 rotation = transform.rotation.eulerAngles;
        // 目標値
        transform.eulerAngles = rotation;
        transform.DORotate(new Vector3(0, 0, -z), speed).SetLoops(-1, LoopType.Yoyo);

        // 進む向き
        PlayerController player = sceneDirector.Player;
        Vector2 dir = player.transform.position - transform.position;
        forward = dir;

        state = State.Alive;
    }

    // TODO: moveEnemy, updateTimer を実装する
    private void moveEnemy()
    {
        throw new System.NotImplementedException();
    }

    private void updateTimer()
    {
        throw new System.NotImplementedException();
    }
}
