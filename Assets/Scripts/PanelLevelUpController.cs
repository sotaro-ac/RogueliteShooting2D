using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelLevelUpController : MonoBehaviour
{
    [SerializeField]
    List<Button> ButtonLevelUps;

    [SerializeField]
    Button buttonCancel;

    GameSceneDirector sceneDirector;

    // 選択カーソル
    int selectButtonCursor;

    // 表示中のボタン
    List<Button> dispButtons;

    public void Init(GameSceneDirector sceneDirector)
    {
        this.sceneDirector = sceneDirector;
    }

    // Update is called once per frame
    void Update() { }

    // レベルアップ時のボタン
    void SetButtonLvUp(Button button, int lv, string name, string desc, Sprite icon)
    {
        Image image = button.transform.Find("ImageItemIcon").GetComponent<Image>();
        Text itemName = button.transform.Find("TextItemName").GetComponent<Text>();
        Text level = button.transform.Find("TextItemLevel").GetComponent<Text>();
        Text info = button.transform.Find("TextItemInfo").GetComponent<Text>();

        image.sprite = icon;
        itemName.text = name;
        info.text = desc;

        // レベルの表示を少し変える
        level.text = "Lv. " + lv;
        level.color = Color.white;

        // 未装備の場合
        if (1 == lv)
        {
            level.text = "NEW!!";
            level.color = Color.yellow;
        }

        button.gameObject.SetActive(true);
    }

    public void OnClickCancel()
    {
        gameObject.SetActive(false);
        sceneDirector.PlayGame();
    }

    // レベルアップパネルを表示
    public void DispPanel(List<WeaponSpawnerStats> items)
    {
        // アイテムがないとき
        buttonCancel.gameObject.SetActive(false);

        // 表示中のボタン
        dispButtons = new List<Button>();

        for (int i = 0; i < ButtonLevelUps.Count; i++)
        {
            // 今回生成するボタン
            Button button = ButtonLevelUps[i];
            // ボタンを初期化
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
            // 表示するアイテムがなければ次へ
            if (items.Count - 1 < i)
                continue;

            // 今回設定するアイテム
            WeaponSpawnerStats item = items[i];

            // ボタンが押された時の処理
            button.onClick.AddListener(() =>
            {
                sceneDirector.PlayGame(new BonusData(item));
                gameObject.SetActive(false);
            });

            // ボタンのデータ
            int lv = item.Lv;
            string name = item.Name;
            string desc = item.Description;
            Sprite icon = item.Icon;

            // ボタン作成
            SetButtonLvUp(button, lv, name, desc, icon);
            dispButtons.Add(button);
        }

        // カーソルをリセット
        selectButtonCursor = 0;

        // 選べるボタンがない場合
        if (1 > items.Count)
        {
            buttonCancel.gameObject.SetActive(true);
            // デフォルトで選択状態にする
            buttonCancel.Select();
        }
        // 1つ目の項目を選択状態にする
        else
        {
            dispButtons[0].Select();
        }

        // 前面に表示
        transform.SetAsLastSibling();

        // パネルを表示
        gameObject.SetActive(true);
    }

    // レベルアップパネルで必要なアイテム数
    public int GetButtonCount()
    {
        return ButtonLevelUps.Count;
    }
}
