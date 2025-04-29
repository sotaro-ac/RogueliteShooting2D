using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneDirector : MonoBehaviour
{
    // スタートボタン
    [SerializeField]
    Button buttonStart;

    // アップグレードボタン
    [SerializeField]
    Button buttonUpgrade;

    // オプションボタン
    [SerializeField]
    Button buttonOption;

    // オプションメニュー
    [SerializeField]
    OptionMenu optionMenu;

    // 左のボタンから順番にID のキャラクターデータを読み込む
    [SerializeField]
    List<Button> buttonPlayers;

    [SerializeField]
    List<int> characterIds;

    [SerializeField]
    Button buttonBack;

    // 選択したキャラクターID
    public static int CharacterId;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // BGMを再生
        SoundController.Instance.PlayBGM(BGM.Title);

        // オプションボタンのイベント設定
        buttonOption.onClick.AddListener(OnClickOption);

        // 戻るボタンのイベント設定
        buttonBack.onClick.AddListener(OnClickBack);

        int idx = 0;
        foreach (var item in buttonPlayers)
        {
            // // 初期表示
            // item.gameObject.SetActive(false);

            // データが足りない
            if (characterIds.Count - 1 < idx)
            {
                break;
            }

            // キャラクターデータ
            int charId = characterIds[idx++];
            CharacterStats charStats = CharacterSettings.Instance.Get(charId);

            // 装備可能な1つ目のデータを表示
            int weaponId = charStats.DefaultWeaponIds[0];
            WeaponSpawnerStats weaponStats = WeaponSpawnerSettings.Instance.Get(weaponId, 1);

            Image imageCharacter = item.transform.Find("ImageCharacter").GetComponent<Image>();
            Image imageWeapon = item.transform.Find("ImageWeapon").GetComponent<Image>();
            Text textName = item.transform.Find("TextName").GetComponent<Text>();

            // WIP: PLAY押下時に表示させる
            Utils.SetAlpha(imageWeapon, 0);
            Utils.SetAlpha(textName, 0);

            if (null != charStats.Prefab)
            {
                imageCharacter.sprite = charStats.Prefab.GetComponent<SpriteRenderer>().sprite;
            }

            // 武器画像
            imageWeapon.sprite = weaponStats.Icon;

            // 名前
            textName.text = charStats.Name;

            // 押された時の処理
            item.onClick.AddListener(() =>
            {
                // BGMを停止
                SoundController.Instance.StopBGM();
                // アニメーションを停止
                DOTween.KillAll();
                // 選択したキャラクターID
                CharacterId = charId;
                // ゲームシーンへ
                SceneManager.LoadScene("GameScene");
            });

            // ボタンを非活性にする
            item.interactable = false;
        }

        // ボタンを選択状態にする
        buttonStart.Select();

        // 戻るボタンを非表示
        Utils.SetAlpha(buttonBack, 0);
        buttonBack.interactable = false;
    }

    // Update is called once per frame
    void Update() { }

    // Startボタン
    public void OnClickStart()
    {
        // スタートボタンフェードアウト
        Utils.DOFadeUpdate(buttonStart, 0, 0.5f);
        buttonStart.interactable = false;

        // WIP: ボタンをコンテナで一括管理
        // スタートボタンフェードアウト
        Utils.DOFadeUpdate(buttonUpgrade, 0, 0.5f);
        buttonUpgrade.interactable = false;

        // スタートボタンフェードアウト
        Utils.DOFadeUpdate(buttonOption, 0, 0.5f);
        buttonOption.interactable = false;

        // 戻るボタンを表示
        Utils.DOFadeUpdate(buttonBack, 1, 0.5f);
        buttonBack.interactable = true;

        // 選択できるプレイヤーをフェードイン
        for (int i = 0; i < buttonPlayers.Count; i++)
        {
            var item = buttonPlayers[i];

            // WIP: PLAY押下時に表示させる
            Image imageWeapon = item.transform.Find("ImageWeapon").GetComponent<Image>();
            Utils.DOFadeUpdate(imageWeapon, 1, 0.5f);
            Text textName = item.transform.Find("TextName").GetComponent<Text>();
            Utils.DOFadeUpdate(textName, 1, 0.5f);

            // ボタンを活性にする
            item.interactable = true;

            // Utils.SetAlpha(item, 0);
            // item.gameObject.SetActive(true);

            // Utils.DOFadeUpdate(item, 1, 1, 0);
        }

        // 最初のボタンを選択状態にする
        buttonPlayers[0].Select();

        SoundController.Instance.PlaySE(SE.Button);
    }

    // オプションボタン
    public void OnClickOption()
    {
        optionMenu.Open();
    }

    // 戻るボタン
    public void OnClickBack()
    {
        // スタートボタンフェードイン
        Utils.DOFadeUpdate(buttonStart, 1, 0.5f);
        buttonStart.interactable = true;

        // アップグレードボタンフェードイン
        Utils.DOFadeUpdate(buttonUpgrade, 1, 0.5f);
        buttonUpgrade.interactable = true;

        // オプションボタンフェードイン
        Utils.DOFadeUpdate(buttonOption, 1, 0.5f);
        buttonOption.interactable = true;

        // 戻るボタンを非表示
        Utils.DOFadeUpdate(buttonBack, 0, 0.5f);
        buttonBack.interactable = false;

        // キャラクター選択をフェードアウト
        for (int i = 0; i < buttonPlayers.Count; i++)
        {
            var item = buttonPlayers[i];
            Image imageWeapon = item.transform.Find("ImageWeapon").GetComponent<Image>();
            Utils.DOFadeUpdate(imageWeapon, 0, 0.5f);
            Text textName = item.transform.Find("TextName").GetComponent<Text>();
            Utils.DOFadeUpdate(textName, 0, 0.5f);

            // ボタンを非活性にする
            item.interactable = false;
        }

        // スタートボタンを選択状態にする
        buttonStart.Select();

        SoundController.Instance.PlaySE(SE.Cancel);
    }

    // シーンが破棄される時に呼ばれる
    private void OnDestroy()
    {
        // BGMを停止
        SoundController.Instance.StopBGM();
    }
}
