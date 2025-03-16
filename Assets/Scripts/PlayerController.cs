using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 移動とAnmation
    Rigidbody2D rigidbody2d;
    Animator animator;
    float moveSpeed = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movePlayer();
    }

    // Playerの移動に関する処理
    void movePlayer()
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
    }
}
