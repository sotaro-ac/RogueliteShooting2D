using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PanelTreasureChestController : MonoBehaviour
{
    [SerializeField]
    Image imageClose;

    [SerializeField]
    Image imageOpen;

    [SerializeField]
    Image imageItem;

    [SerializeField]
    Image imageBackFX;

    [SerializeField]
    Image imageBackFXShiny;

    [SerializeField]
    Button buttonOpen;

    [SerializeField]
    Button buttonClose;

    [SerializeField]
    Text textDescription;

    GameSceneDirector sceneDirector;

    // 取得アイテム
    ItemData itemData;

    // 取得アイテム画像初期位置
    Vector3 imageItemInitPos;

    // 宝箱画像初期スケール
    Vector3 imageCloseInitScale;

    // 取得アイテムアニメーション
    Vector2 itemTargetPos = new(0, 70);

    public void Init(GameSceneDirector sceneDirector)
    {
        this.sceneDirector = sceneDirector;

        // 初期位置
        imageItemInitPos = imageItem.rectTransform.anchoredPosition;
        imageCloseInitScale = imageClose.rectTransform.localScale;
    }

    // 宝箱パネルを表示
    public void DispPanel(ItemData item)
    {
        // 今回取得したアイテム
        itemData = item;

        // 場所など初期値
        imageItem.rectTransform.anchoredPosition = imageItemInitPos;
        imageClose.rectTransform.localScale = imageCloseInitScale;
        imageClose.rectTransform.localEulerAngles = Vector3.zero;
        imageBackFX.rectTransform.anchoredPosition = itemTargetPos;
        imageBackFXShiny.rectTransform.anchoredPosition = itemTargetPos;

        // アイテム画像
        imageItem.sprite = item.Icon;
        Utils.SetAlpha(imageItem, 0);

        // アイテム説明
        textDescription.text = item.Description;
        Utils.SetAlpha(textDescription, 0);

        // 閉じた宝箱
        imageClose.gameObject.SetActive(true);
        Utils.SetAlpha(imageClose, 1);

        // 開いた宝箱
        imageOpen.gameObject.SetActive(false);
        Utils.SetAlpha(imageOpen, 1);

        // オープンボタン
        buttonOpen.gameObject.SetActive(true);
        Utils.SetAlpha(buttonOpen, 1);

        // クローズボタン
        buttonClose.gameObject.SetActive(false);
        Utils.SetAlpha(buttonClose, 0);

        // 演出
        Utils.SetAlpha(imageBackFX, 0);
        Utils.SetAlpha(imageBackFXShiny, 0);

        // ボタンを選択状態にする
        buttonOpen.Select();

        // パネル本体
        gameObject.SetActive(true);
    }

    // オープンボタンをクリック
    public void OnClickOpen()
    {
        // ボタン非表示
        buttonOpen.gameObject.SetActive(false);

        // 閉じた宝箱
        Transform image = imageClose.transform;

        // アニメーション設定
        Vector3 punchScale = new(1.5f, 1.5f, 0);
        Vector3 punchRotate = new(0, 0, 30f);
        Vector3 endScale = new(1.5f, 0.5f, 0);
        float duration = 0.5f;

        // シーケンスでアニメーションを順番に再生
        Sequence seq = DOTween.Sequence();

        // 宝箱が弾む（大きさ、時間、振動数、弾力性）
        seq.Append(image.DOPunchScale(punchScale, duration, 5, 1));
        seq.Join(
            image
                .DOPunchRotation(punchRotate, duration)
                // アニメーションの始まりと終わりがゆっくり、中間が速くなる
                .SetEase(Ease.InOutQuad)
                // 繰り返す回数とタイプ
                .SetLoops(1, LoopType.Yoyo)
        );

        // 潰れるアニメーション（Appendで前のアニメーションが終わってから再生）
        seq.Append(image.DOScale(endScale, duration).SetEase(Ease.OutBounce));

        // 再生
        seq.Play().SetUpdate(true).OnComplete(() => DispItem());
    }

    // 回転のみ関数化
    void DoRotateLoops(Image image, int dir = 1)
    {
        image
            .transform.DORotate(new Vector3(0, 0, 360f) * dir, 60f, RotateMode.FastBeyond360)
            .SetUpdate(true)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    // アイテム表示
    void DispItem()
    {
        // 宝箱表示
        imageClose.gameObject.SetActive(false);
        imageOpen.gameObject.SetActive(true);

        // 取得アイテム
        RectTransform image = imageItem.rectTransform;

        // アイテム表示時間
        float itemDuration = 1f;
        // 演出表示時間
        float fxDuration = itemDuration / 2;

        // アニメーションを順番に再生
        Sequence seq = DOTween.Sequence();

        // 開いた宝箱がフェードアウト（全体の開始を遅らせる）
        seq.Append(imageOpen.DOFade(0, itemDuration).SetDelay(0.5f));

        // アイテムフェードイン＆移動
        seq.Append(imageItem.DOFade(1, itemDuration));
        seq.Join(image.DOAnchorPos(itemTargetPos, itemDuration));

        // 演出フェードイン
        seq.Append(imageBackFX.DOFade(1, fxDuration));
        seq.Join(imageBackFXShiny.DOFade(0.8f, fxDuration));

        // 説明フェードイン
        seq.Append(
            textDescription
                .DOFade(1, fxDuration)
                .OnComplete(() => buttonClose.gameObject.SetActive(true))
        );

        // 閉じるボタンと子オブジェクトをフェードイン
        seq.Append(buttonClose.image.DOFade(1, fxDuration).OnComplete(() => buttonClose.Select()));
        foreach (var item in buttonClose.GetComponentsInChildren<Graphic>())
        {
            seq.Join(item.DOFade(1, fxDuration));
        }

        // 開始
        seq.Play().SetUpdate(true);

        // 無限ループ系はシーケンスと別で動かす
        DoRotateLoops(imageBackFX);
        DoRotateLoops(imageBackFXShiny, -1);
    }

    public void OnClickClose()
    {
        // ループ系を止める
        imageBackFX.DOKill();
        imageBackFXShiny.DOKill();
        // パネルを非表示
        gameObject.SetActive(false);
        // ゲーム再開
        sceneDirector.PlayGame(new BonusData(itemData));
    }
}
