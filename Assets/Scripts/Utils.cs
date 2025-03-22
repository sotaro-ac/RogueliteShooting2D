using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

// ゲームで使う共通処理をまとめたクラス
public static class Utils
{
    public static string GetTextTimer(float timer)
    {
        int seconds = (int)timer % 60;
        int minutes = (int)timer / 60;
        return minutes.ToString() + ":" + seconds.ToString("00");
    }

    // 当たり判定のあるタイルかどうか調べる
    public static bool IsColliderTile(Tilemap tilemapCollider, Vector2 position)
    {
        // セル位置に変換
        Vector3Int cellPosition = tilemapCollider.WorldToCell(position);

        // 当たり判定あり
        if (tilemapCollider.GetTile(cellPosition))
        {
            return true;
        }
        return false;
    }
}
