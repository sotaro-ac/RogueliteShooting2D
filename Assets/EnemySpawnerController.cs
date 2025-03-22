using System;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnType
{
    Normal,
    Group,
}

[Serializable]
public class EnemySpawnData
{
    // 説明用
    public string Title;

    // 出現経過時間
    public int ElapsedMinutes;
    public int ElapsedSeconds;

    // 出現タイプ
    public SpawnType SpawnType;

    // 生成時間
    public float SpawnDuration;

    // 生成数
    public int SpawnCountMax;

    // 生成する敵ID
    public List<int> EnemyIds;
}

public class EnemySpawnerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
