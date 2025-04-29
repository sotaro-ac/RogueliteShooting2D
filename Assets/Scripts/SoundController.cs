using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum BGM
{
    Title = 0,
    Game = 1,
    GameOver = 2,
}

public enum SE
{
    Button = 0,
    Select = 1,
    Cancel = 2,
    Damage = 3,
    Item = 4,
    LevelUp = 5,
    Slash = 6,
}

public class SoundController : MonoBehaviour
{
    // **Feature**: インスペクター上の配列にEnum名で表示させて見やすくする
    //  e.g. Element 0, 1, 2, ... --> Title, Game, ...
    // - NamedArrayAttribute: 配列要素にEnum名を関連付けるための属性
    // - NamedArrayDrawer: インスペクターでの配列表示をEnum名で行うためのカスタムプロパティドロワー
    // > Ref: https://qiita.com/miyumiyu/items/eee6cfbdc6078270450c

    // ===
    // NamedArrayAttribute用のカスタムプロパティドロワークラス
    // ===
    [CustomPropertyDrawer(typeof(NamedArrayAttribute))]
    internal class NamedArrayDrawer : PropertyDrawer
    {
        // インスペクターでのGUI描画処理をオーバーライド
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // 属性からNamedArrayAttributeを取得
            var namedArrayAttribute = (NamedArrayAttribute)attribute;
            // 対象のEnum型から名前の配列を取得
            var names = System.Enum.GetNames(namedArrayAttribute.TargetEnum);

            // プロパティパスに"data"が含まれる場合(配列要素の場合)
            if (property.propertyPath.Contains("data"))
            {
                // プロパティパスから配列のインデックスを取得
                int index = int.Parse(property.propertyPath.Split('[', ']')[1]);
                // インデックスに対応するEnum名をラベルとしてプロパティフィールドを描画
                EditorGUI.PropertyField(rect, property, new GUIContent(names[index]), true);
            }
            else
            {
                // 通常のプロパティフィールドとして描画
                EditorGUI.PropertyField(rect, property, label, true);
            }
        }
    }

    // ===
    // 配列要素にEnum名を関連付けるための属性クラス
    // ===
    internal class NamedArrayAttribute : PropertyAttribute
    {
        // 関連付けるEnum型を保持するプロパティ
        public System.Type TargetEnum { get; private set; }

        // コンストラクタ - 関連付けるEnum型を受け取る
        public NamedArrayAttribute(System.Type targetEnum)
        {
            // 受け取ったEnum型を保持する
            TargetEnum = targetEnum;
        }
    }

    private static SoundController instance;
    private static readonly object lockObject = new object();

    public static SoundController Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        var go = new GameObject("SoundController");
                        instance = go.AddComponent<SoundController>();
                        DontDestroyOnLoad(go);
                    }
                }
            }
            return instance;
        }
    }

    [SerializeField]
    private AudioSource bgmSource;

    [SerializeField]
    private AudioSource seSource;

    [SerializeField]
    [NamedArray(typeof(BGM))]
    private AudioClip[] bgmClips;

    [SerializeField]
    [NamedArray(typeof(SE))]
    private AudioClip[] seClips;

    [SerializeField]
    private float bgmVolume = 0.75f;

    [SerializeField]
    private float seVolume = 0.75f;

    public float BGMVolume
    {
        get => bgmVolume;
        set
        {
            bgmVolume = value;
            bgmSource.volume = value;
        }
    }

    public float SEVolume
    {
        get => seVolume;
        set
        {
            seVolume = value;
            seSource.volume = value;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        seSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
        seSource.volume = seVolume;
    }

    public void PlayBGM(BGM bgm)
    {
        bgmSource.clip = bgmClips[(int)bgm];
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySE(SE se)
    {
        seSource.PlayOneShot(seClips[(int)se]);
    }
}
