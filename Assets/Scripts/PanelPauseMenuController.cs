using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelPauseMenuController : MonoBehaviour
{
    GameSceneDirector sceneDirector;

    // 初期化
    public void Init(GameSceneDirector sceneDirector)
    {
        this.sceneDirector = sceneDirector;
    }

    // Update is called once per frame
    void Update()
    {
        // ポーズ画面
        // TODO: GameSceneDirector の Input System の導入に合わせて削除する
        if (Input.GetKeyDown(KeyCode.Escape) && isActiveAndEnabled == true)
        {
            HidePanel();
            sceneDirector.SetEnabled(true);
        }
    }

    // パネル表示
    public void DispPanel()
    {
        // 最前面に表示
        transform.SetAsLastSibling();

        // パネル表示
        gameObject.SetActive(true);
    }

    // パネル表示
    public void HidePanel()
    {
        // パネル非表示
        gameObject.SetActive(false);
    }
}
