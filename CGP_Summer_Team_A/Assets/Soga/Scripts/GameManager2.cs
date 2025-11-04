using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; 
using System.Collections;
using UnityEngine.SceneManagement; 

public class GameManager2 : MonoBehaviour
{
    // ★★★ 役満勝利回数の管理 (static変数でシーンをまたぐ想定) ★★★
    public static int yakumanWinCount = 0;
    private const int YAKUMAN_WIN_LIMIT = 3; 

    // ★★★ ゲーム終了管理用の既存フィールド ★★★
    [Header("01. ゲームコアと終了管理")]
    public PlayerMove playerMove;
    public AudioSource raceBGM;
    public AudioSource countdownSE;
    
    [Tooltip("HierarchyのNPCオブジェクトにアタッチされているNPCplayerスクリプトをすべてここに設定します。")]
    public NPCplayer[] npcMoveScripts; 

    public Button agariButton; 
    public Shooter2D scoreManager; 
    
    [Header("02. Debug Hand Selection (Set only ONE to true)")]
    public bool forceDaisangenHand = false; 
    public bool forceSuuankouHand = false; 
    public bool forceKokushiHand = false;
    public bool forceDaisushiHand = false;
    public bool forceChinroutouHand = false;
    public bool forceRyuuisoHand = false;
    public bool forceTsuisoHand = false;
    public bool forceShosushiHand = false;
    public bool forceChuurenHand = false; 

    [Header("03. リザルト画面設定")]
    public GameObject ResultPanel; // 役満勝利時に表示するパネル
    public Image yakumanImage;
    public GameObject ResultPanel3; 
    public Image goalImage; 
    

    [Header("04. シーン遷移とリザルトボタン (要Inspector設定)")]
    // ★★★ Title Scene Nameはここにあります ★★★
    public string titleSceneName = "TitleScene"; 
    public Button nextGameButton;      // 次のゲームに進むボタン
    public Button backToTitleButton;   // タイトルに戻るボタン (3回達成時)
    
    // ★★★ 不要な Restart Buttonフィールドは完全に削除済み ★★★

    [Header("05. スプライト設定")]
    [SerializeField] private Sprite kokushiSprite;
    [SerializeField] private Sprite daisangenSprite;
    [SerializeField] private Sprite suuankouSprite;
    [SerializeField] private Sprite daisushiSprite;
    [SerializeField] private Sprite chinroutouSprite;
    [SerializeField] private Sprite ryuuisoSprite;
    [SerializeField] private Sprite tsuisoSprite;
    [SerializeField] private Sprite shosushiSprite;
    [SerializeField] private Sprite chuurenSprite;
    [SerializeField] private Sprite goalSprite; 
    
    void Start()
    {
        StartCoroutine(InitializeAfterFrames());
    }

    IEnumerator InitializeAfterFrames()
    {
        yield return null;
        
        // ★★★ 修正点1: リザルトパネルをコードで非表示にする (ゲーム開始時) ★★★
        if (ResultPanel != null)
        {
            ResultPanel.SetActive(false); 
        }

        // 参照がInspectorで設定されていない場合の自動検索（補助的な機能）
        if (playerMove == null)
        {
            playerMove = FindFirstObjectByType<PlayerMove>();
        }
        if (raceBGM == null) raceBGM = GameObject.Find("RaceBGM")?.GetComponent<AudioSource>();
        if (countdownSE == null) countdownSE = GameObject.Find("CountdownSE")?.GetComponent<AudioSource>();

        // ★★★ 修正点2: ボタンのリスナー設定と初期状態を非表示にする ★★★
        if (nextGameButton != null)
        {
            nextGameButton.onClick.AddListener(OnNextGameButton);
            nextGameButton.gameObject.SetActive(false); // 初期状態を非表示にする
        }
        if (backToTitleButton != null)
        {
            backToTitleButton.onClick.AddListener(OnBackToTitleButton);
            backToTitleButton.gameObject.SetActive(false); // 初期状態を非表示にする
        }
        
        // --- テスト配牌ロジック ---
        if (forceDaisangenHand)
        {
            SetDaisangenHand();
        }
        else if (forceSuuankouHand)
        {
            SetSuuankouHand();
        }
        else if (forceKokushiHand)
        {
            SetKokushiHand();
        }
        else if (forceDaisushiHand)
        {
            SetDaisushiHand();
        }
        else if (forceChinroutouHand)
        {
            SetChinroutouHand();
        }
        else if (forceRyuuisoHand)
        {
            SetRyuuisoHand();
        }
        else if (forceTsuisoHand)
        {
            SetTsuisoHand();
        }
        else if (forceShosushiHand) 
        {
            SetShosushiHand();
        }
        else if (forceChuurenHand) 
        {
            SetChuurenHand();
        }
        
        if (agariButton != null)
        {
            agariButton.onClick.AddListener(OnAgariButton);
            Debug.Log("和了ボタンが設定されました。");
        }
        else
        {
            Debug.LogError("和了ボタンが設定されていません。");
        }
    }
    // ------------------------------------

    // ====================================================================
    // ★★★ テスト配牌メソッド群 ★★★
    // ====================================================================
    
    private void SetDaisangenHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Honor, 5), (Suit.Honor, 5), (Suit.Honor, 5), 
            (Suit.Honor, 6), (Suit.Honor, 6), (Suit.Honor, 6), 
            (Suit.Honor, 7), (Suit.Honor, 7), (Suit.Honor, 7), 
            (Suit.Pinzu, 2), (Suit.Pinzu, 2), (Suit.Pinzu, 2), 
            (Suit.Pinzu, 4), (Suit.Pinzu, 4) 
        };
        PassHandToManager(handData, "大三元");
    }
    private void SetSuuankouHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Manzu, 1), (Suit.Manzu, 1), (Suit.Manzu, 1), (Suit.Manzu, 1), 
            (Suit.Pinzu, 5), (Suit.Pinzu, 5), (Suit.Pinzu, 5), (Suit.Pinzu, 5), 
            (Suit.Souzu, 9), (Suit.Souzu, 9), (Suit.Souzu, 9), (Suit.Souzu, 9), 
            (Suit.Honor, 1), (Suit.Honor, 1) 
        };
        PassHandToManager(handData, "四暗刻");
    }

    private void SetKokushiHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Manzu, 1), (Suit.Manzu, 9), 
            (Suit.Pinzu, 1), (Suit.Pinzu, 9), 
            (Suit.Souzu, 1), (Suit.Souzu, 9), 
            (Suit.Honor, 1), (Suit.Honor, 2), (Suit.Honor, 3), (Suit.Honor, 4), 
            (Suit.Honor, 5), (Suit.Honor, 6), (Suit.Honor, 7), 
            (Suit.Manzu, 1) 
        };
        PassHandToManager(handData, "国士無双");
    }

    private void SetDaisushiHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Honor, 1), (Suit.Honor, 1), (Suit.Honor, 1), 
            (Suit.Honor, 2), (Suit.Honor, 2), (Suit.Honor, 2), 
            (Suit.Honor, 3), (Suit.Honor, 3), (Suit.Honor, 3), 
            (Suit.Honor, 4), (Suit.Honor, 4), (Suit.Honor, 4), 
            (Suit.Pinzu, 5), (Suit.Pinzu, 5) 
        };
        PassHandToManager(handData, "大四喜");
    }

    private void SetChinroutouHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Manzu, 1), (Suit.Manzu, 1), (Suit.Manzu, 1), 
            (Suit.Manzu, 9), (Suit.Manzu, 9), (Suit.Manzu, 9), 
            (Suit.Pinzu, 1), (Suit.Pinzu, 1), (Suit.Pinzu, 1), 
            (Suit.Pinzu, 9), (Suit.Pinzu, 9), (Suit.Pinzu, 9), 
            (Suit.Souzu, 1), (Suit.Souzu, 1) 
        };
        PassHandToManager(handData, "清老頭");
    }

    private void SetRyuuisoHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Souzu, 2), (Suit.Souzu, 2), (Suit.Souzu, 2), 
            (Suit.Souzu, 3), (Suit.Souzu, 4), (Suit.Souzu, 5), 
            (Suit.Souzu, 6), (Suit.Souzu, 6), (Suit.Souzu, 6), 
            (Suit.Souzu, 8), (Suit.Souzu, 8), (Suit.Souzu, 8), 
            (Suit.Honor, 6), (Suit.Honor, 6) // 發・發
        };
        PassHandToManager(handData, "緑一色");
    }

    private void SetTsuisoHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Honor, 1), (Suit.Honor, 1), (Suit.Honor, 1), 
            (Suit.Honor, 2), (Suit.Honor, 2), (Suit.Honor, 2), 
            (Suit.Honor, 3), (Suit.Honor, 3), (Suit.Honor, 3), 
            (Suit.Honor, 4), (Suit.Honor, 4), (Suit.Honor, 4), 
            (Suit.Honor, 5), (Suit.Honor, 5) // 白
        };
        PassHandToManager(handData, "字一色");
    }

    private void SetShosushiHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Honor, 1), (Suit.Honor, 1), (Suit.Honor, 1), 
            (Suit.Honor, 2), (Suit.Honor, 2), (Suit.Honor, 2), 
            (Suit.Honor, 3), (Suit.Honor, 3), (Suit.Honor, 3), 
            (Suit.Honor, 4), (Suit.Honor, 4), // 北 (雀頭)
            (Suit.Pinzu, 5), (Suit.Pinzu, 6), (Suit.Pinzu, 7) 
        };
        PassHandToManager(handData, "小四喜");
    }

    private void SetChuurenHand() 
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Manzu, 1), (Suit.Manzu, 1), (Suit.Manzu, 1), 
            (Suit.Manzu, 2), (Suit.Manzu, 3), (Suit.Manzu, 4), 
            (Suit.Manzu, 5), (Suit.Manzu, 6), (Suit.Manzu, 7), 
            (Suit.Manzu, 8), (Suit.Manzu, 9), (Suit.Manzu, 9), 
            (Suit.Manzu, 9), (Suit.Pinzu, 1) 
        };
        PassHandToManager(handData, "九蓮宝燈");
    }
    
    private void PassHandToManager(List<(Suit suit, int rank)> handData, string name)
    {
        if (MahjongManager.instance == null)
        {
             Debug.LogError($"【{name}】配牌失敗: MahjongManager.instance が null です。");
             return;
        }

        MahjongManager.instance.SetTestHand(handData);
        Debug.Log($"【実験モード】{name}をMahjongManagerに設定しました。");
    }


    // ====================================================================
    // ★★★ OnAgariButton メソッド (リザルト表示とボタン制御のロジック) ★★★
    // ====================================================================
    public void OnAgariButton()
    {
        if (MahjongManager.instance == null)
        {
            Debug.LogError("MahjongManager.instance が null です。");
            return;
        }

        const int YAKUMAN_SCORE = 32000;
        var myHand = new List<Tile>(MahjongManager.instance.playerHand);

        // --- 役満判定とゲーム終了 ---
        bool isYakuman = false;
        Sprite spriteToShow = null;

        // 役満判定ロジック (全て記述)
        if (ShosushiChecker.IsShosushi(myHand))
        {
            isYakuman = true; Debug.Log("🎉 小四喜です！"); scoreManager?.AddScore(YAKUMAN_SCORE); spriteToShow = shosushiSprite;
        }
        else if (TsuisoChecker.IsTsuiso(myHand))
        {
            isYakuman = true; Debug.Log("🎉 字一色です！"); scoreManager?.AddScore(YAKUMAN_SCORE); spriteToShow = tsuisoSprite;
        }
        else if (RyuuisoChecker.IsRyuuiso(myHand))
        {
            isYakuman = true; Debug.Log("🎉 緑一色です！"); scoreManager?.AddScore(YAKUMAN_SCORE); spriteToShow = ryuuisoSprite;
        }
        else if (DaisushiChecker.IsDaisushi(myHand))
        {
            isYakuman = true; Debug.Log("🎉 大四喜です！"); scoreManager?.AddScore(YAKUMAN_SCORE * 2); spriteToShow = daisushiSprite;
        }
        else if (ChinroutouChecker.IsChinroutou(myHand))
        {
            isYakuman = true; Debug.Log("🎉 清老頭です！"); scoreManager?.AddScore(YAKUMAN_SCORE); spriteToShow = chinroutouSprite;
        }
        else if (KokushiChecker.IsKokushi(myHand))
        {
            isYakuman = true; Debug.Log("🎉 国士無双です！"); scoreManager?.AddScore(YAKUMAN_SCORE); spriteToShow = kokushiSprite;
        }
        else if (DaisangenChecker.IsDaisangen(myHand))
        {
            isYakuman = true; Debug.Log("🎉 大三元です！"); scoreManager?.AddScore(YAKUMAN_SCORE); spriteToShow = daisangenSprite;
        }
        else if (SuuankouChecker.IsSuuankou(myHand))
        {
            isYakuman = true; Debug.Log("🎉 四暗刻です！"); scoreManager?.AddScore(YAKUMAN_SCORE); spriteToShow = suuankouSprite;
        }
        else if (ChuurenChecker.IsChuuren(myHand)) 
        {
            isYakuman = true; Debug.Log("🎉 九蓮宝燈です！"); scoreManager?.AddScore(YAKUMAN_SCORE); spriteToShow = chuurenSprite;
        }

        //役満ではない時
        if (!isYakuman)
        {
            Debug.Log("役満ではありません。（他の役の判定は未実装です）");
            return;
        }

        // ----------------------------------------------------
        // 役満が成立した場合の処理
        yakumanWinCount++; 
        Debug.Log($"現在の役満勝利回数: {yakumanWinCount} / {YAKUMAN_WIN_LIMIT}");
        
        if (yakumanImage != null && spriteToShow != null)
        {
            yakumanImage.sprite = spriteToShow;
            yakumanImage.gameObject.SetActive(true);
        }

        GameOver();

        // 4. リザルト画面を表示し、ボタンを動的に切り替える
        if (ResultPanel != null)
        {
            // リザルトパネルを表示
            ResultPanel.SetActive(true); 

            if (yakumanWinCount >= YAKUMAN_WIN_LIMIT)
            {
                // 3回達成時: タイトルへ戻るボタンを表示
                Debug.Log($"🎉 3回目の役満達成！タイトルへ戻るボタンを表示します。");
                if (nextGameButton != null) nextGameButton.gameObject.SetActive(false);
                if (backToTitleButton != null) backToTitleButton.gameObject.SetActive(true);
            }
            else 
            {
                // 1回/2回達成時: 次のゲームへ進むボタンを表示 (点数は引き継ぎ)
                Debug.Log("役満達成！規定回数に達していません。次のゲームへ進むボタンを表示します。");
                if (nextGameButton != null) nextGameButton.gameObject.SetActive(true);
                if (backToTitleButton != null) backToTitleButton.gameObject.SetActive(false);
            }
        }
        else 
        {
             Debug.LogError("ResultPanelが設定されていません。ボタン制御ができません。");
        }
        // ----------------------------------------------------
    }

    // ====================================================================
    // ★★★ OnNextGameButton: 次のゲームに進む (点数引き継ぎ) ★★★
    // ====================================================================
    public void OnNextGameButton()
    {
        if (ResultPanel != null)
        {
            ResultPanel.SetActive(false);
        }
        
        Debug.Log($"次のゲームに進みます。現在のスコア: {yakumanWinCount} を引き継ぎます。");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // ====================================================================
    // ★★★ OnBackToTitleButton: タイトルに戻る (点数リセット) ★★★
    // ====================================================================
    public void OnBackToTitleButton()
    {
        yakumanWinCount = 0; 
        Debug.Log("タイトルに戻ります。役満勝利回数をリセットしました。");

        if (!string.IsNullOrEmpty(titleSceneName))
        {
            SceneManager.LoadScene(titleSceneName);
        }
        else
        {
            Debug.LogError("タイトルシーン名(titleSceneName)が設定されていません。");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // ====================================================================
    // ★★★ ゲーム終了メソッド群 ★★★
    // ====================================================================
    public void GameOver()
    {
        if (playerMove != null)
        {
            playerMove.enabled = false;
        }
        if (raceBGM != null && raceBGM.isPlaying)
        {
            raceBGM.Stop();
        }
        if (countdownSE != null && countdownSE.isPlaying)
        {
            countdownSE.Stop();
        }
        if (npcMoveScripts != null)
        {
            foreach (var script in npcMoveScripts)
            {
                if (script != null)
                {
                    // script.StopMovement(); // NPCの移動停止メソッドを想定
                }
            }
        }
    }

    public void OnGoal(string characterName)
    {
        if (MahjongManager.instance.roundOver) return;
        MahjongManager.instance.roundOver = true;

        Debug.Log($"{characterName}がゴールしました。流局です。");

        if (goalImage != null && goalSprite != null)
        {
            goalImage.sprite = goalSprite;
            goalImage.gameObject.SetActive(true);
        }

        if (ResultPanel3 != null)
        {
            ResultPanel3.SetActive(true);
        }
        else
        {
            Debug.LogError("ResultPanel3が設定されていません。");
        }

        GameOver();
    }
}