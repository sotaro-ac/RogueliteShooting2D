using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

// ゲームで使う共通処理をまとめたクラス
public static class Utils
{
    public static string GetTextTimer(float timer)
    {
        int seconds = (int)timer % 60;
        int minutes = (int)timer / 60;
        return minutes.ToString() + ":" + seconds.ToString("00");
    }
}
