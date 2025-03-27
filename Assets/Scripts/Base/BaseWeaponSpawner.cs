using UnityEngine;

public class BaseWeaponSpawner : MonoBehaviour
{
    // 武器のprefab
    [SerializeField]
    GameObject PrefabWeapon;

    // 武器データ
    public WeaponSpawnerStats Stats;

    // 与えた総ダメージ
    public float TotalDamage;

    // 稼働タイマー
    public float TotalTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
