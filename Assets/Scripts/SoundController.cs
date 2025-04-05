using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    // シングルトン
    public static SoundController Instance;

    // 再生装置
    AudioSource audioSource;

    // BGM
    [SerializeField]
    List<AudioClip> audioClipsBGM;

    // SE
    [SerializeField]
    List<AudioClip> audioClipsSE;

    private void Awake()
    {
        if (null == Instance)
        {
            // もし存在しなければセットする
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;

            // オブジェクトをセットする
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        // 2回目以降に生成されたオブジェクトは削除する
        else
        {
            Destroy(this.gameObject);
        }
    }

    // BGM再生
    public void PlayBGM(int index)
    {
        audioSource.clip = audioClipsBGM[index];
        audioSource.Play();
    }

    // SE再生
    public void PlaySE(int index)
    {
        audioSource.PlayOneShot(audioClipsSE[index]);
    }
}
