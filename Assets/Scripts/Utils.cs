using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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

    // アルファ値設定
    public static void SetAlpha(Graphic graphic, float alpha)
    {
        // 元のカラー
        Color color = graphic.color;

        // アルファ値設定
        color.a = alpha;
        graphic.color = color;
    }

    // アルファ値設定（ボタン）
    public static void SetAlpha(Button button, float alpha)
    {
        // ボタン自体
        SetAlpha(button.image, alpha);

        // 子オブジェクト全て
        foreach (var item in button.GetComponentsInChildren<Graphic>())
        {
            SetAlpha(item, alpha);
        }
    }
}
