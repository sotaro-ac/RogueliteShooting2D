using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup panelOptionMenu;

    [SerializeField]
    private Button buttonBack;

    [SerializeField]
    private Slider sliderBGM;

    [SerializeField]
    private Slider sliderSE;

    [SerializeField]
    private CanvasGroup panelTitle;

    [SerializeField]
    private CanvasGroup panelCharacters;

    private void Start()
    {
        // 初期状態は非表示
        panelOptionMenu.alpha = 0;
        panelOptionMenu.interactable = false;
        panelOptionMenu.blocksRaycasts = false;

        // 閉じるボタンのイベント設定
        buttonBack.onClick.AddListener(Close);

        // スライダーの初期値設定
        sliderBGM.value = SoundController.Instance.BGMVolume;
        sliderSE.value = SoundController.Instance.SEVolume;

        // スライダーのイベント設定
        sliderBGM.onValueChanged.AddListener(OnBGMVolumeChanged);
        sliderSE.onValueChanged.AddListener(OnSEVolumeChanged);
    }

    public void Open()
    {
        // メニューを表示
        panelOptionMenu.interactable = true;
        panelOptionMenu.blocksRaycasts = true;
        panelOptionMenu.DOFade(1, 0.5f);

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
        panelOptionMenu.interactable = false;
        panelOptionMenu.blocksRaycasts = false;
        panelOptionMenu.DOFade(0, 0.5f);

        // タイトル要素を表示
        panelTitle.alpha = 1;
        panelTitle.interactable = true;
        panelTitle.blocksRaycasts = true;

        // キャラクター選択を有効化
        panelCharacters.interactable = true;
        panelCharacters.blocksRaycasts = true;

        SoundController.Instance.PlaySE(SE.Cancel);
    }

    private void OnBGMVolumeChanged(float value)
    {
        SoundController.Instance.BGMVolume = value;
    }

    private void OnSEVolumeChanged(float value)
    {
        SoundController.Instance.PlaySE(SE.Item);
        SoundController.Instance.SEVolume = value;
    }
}
