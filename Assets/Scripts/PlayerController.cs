using System.Collections;
using System.Collections.Generic;
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
}
