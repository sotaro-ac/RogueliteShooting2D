using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    public static UpgradeMenu Instance;

    [SerializeField]
    private CanvasGroup panelUpgradeMenu;

    [SerializeField]
    private Button buttonBack;

    [SerializeField]
    private CanvasGroup panelTitle;

    [SerializeField]
    private CanvasGroup panelCharacters;

    private void Start()
    {
        if (null == Instance)
        {
            Instance = this;
        }

        // 初期状態は非表示
        panelUpgradeMenu.alpha = 0;
        panelUpgradeMenu.interactable = false;
        panelUpgradeMenu.blocksRaycasts = false;

        // 閉じるボタンのイベント設定
        buttonBack.onClick.AddListener(Close);
    }

    public void Open()
    {
        // メニューを表示
        panelUpgradeMenu.interactable = true;
        panelUpgradeMenu.blocksRaycasts = true;
        panelUpgradeMenu.DOFade(1, 0.5f);

        // タイトル要素を非表示
        panelTitle.alpha = 0;
        panelTitle.interactable = false;
        panelTitle.blocksRaycasts = false;

        // キャラクター選択を無効化
        panelCharacters.interactable = false;
        panelCharacters.blocksRaycasts = false;

        // 閉じるボタンを選択状態にする
        buttonBack.Select();

        SoundController.Instance.PlaySE(SE.Button);
    }

    public void Close()
    {
        // メニューを非表示
        panelUpgradeMenu.interactable = false;
        panelUpgradeMenu.blocksRaycasts = false;
        panelUpgradeMenu.DOFade(0, 0.5f);

        // タイトル要素を表示
        panelTitle.DOFade(1, 0.5f);
        panelTitle.interactable = true;
        panelTitle.blocksRaycasts = true;

        // キャラクター選択を有効化
        panelCharacters.interactable = true;
        panelCharacters.blocksRaycasts = true;

        SoundController.Instance.PlaySE(SE.Cancel);
    }
}
