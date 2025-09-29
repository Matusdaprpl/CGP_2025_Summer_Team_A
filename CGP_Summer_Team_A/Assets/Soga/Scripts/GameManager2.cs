using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; 
using System.Collections; 

public class GameManager2 : MonoBehaviour
{
    // ★★★ ゲーム終了管理用のフィールド ★★★
    [Header("ゲーム終了管理")]
    public PlayerMove playerMove;    // プレイヤーの車を停止させる
    public AudioSource raceBGM;      // BGM停止用
    public AudioSource countdownSE;  // SE停止用
    
    [Tooltip("HierarchyのNPCオブジェクトにアタッチされているNPCplayerスクリプトをすべてここに設定します。")]
    public NPCplayer[] npcMoveScripts; // NPCplayer型に修正済み

    public Button agariButton; 
    
    [Header("スコア管理")]
    public Shooter2D scoreManager; 
    
    [Header("Debug Hand Selection (Set only ONE to true)")]
    public bool forceDaisangenHand = false; 
    public bool forceSuuankouHand = false; 
    public bool forceKokushiHand = false;
    public bool forceSuukantsuHand = false;
    public bool forceDaisushiHand = false;
    public bool forceChinroutouHand = false;
    public bool forceRyuuisoHand = false;
    public bool forceTsuisoHand = false;
    public bool forceShosushiHand = false;

    [Header("リザルト画面設定")]
    public GameObject ResultPanel1;
    
    public GameObject ResultPanel;

    // MahjongUIManager の初期化遅延に対応するため、コルーチンで実行
    void Start()
    {
        StartCoroutine(InitializeAfterFrames());
    }

    IEnumerator InitializeAfterFrames()
    {
        yield return null;

        // 参照がInspectorで設定されていない場合の自動検索（補助的な機能）
        if (playerMove == null)
        {
            playerMove = FindFirstObjectByType<PlayerMove>();
        }
        if (raceBGM == null) raceBGM = GameObject.Find("RaceBGM")?.GetComponent<AudioSource>();
        if (countdownSE == null) countdownSE = GameObject.Find("CountdownSE")?.GetComponent<AudioSource>();


        // --- テスト配牌ロジック ---
        if (forceDaisangenHand)
        {
            SetDaisangenHand();
        }
        else if (forceSuukantsuHand)
        {
            SetSuukantsuHand();
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
    // ★★★ テスト配牌メソッド群 (省略) ★★★
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

    private void SetSuukantsuHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Manzu, 1), (Suit.Manzu, 1), (Suit.Manzu, 1), 
            (Suit.Pinzu, 2), (Suit.Pinzu, 2), (Suit.Pinzu, 2), 
            (Suit.Souzu, 3), (Suit.Souzu, 3), (Suit.Souzu, 3), 
            (Suit.Honor, 4), (Suit.Honor, 4), (Suit.Honor, 4), 
            (Suit.Pinzu, 5), (Suit.Pinzu, 5) 
        };
        PassHandToManager(handData, "四槓子");
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

    public void OnAgariButton()
    {
        Debug.Log("和了ボタンが押されました。");
        if (MahjongManager.instance == null)
        {
            Debug.LogError("MahjongManager.instance が null です。");
            return;
        }
        
        const int YAKUMAN_SCORE = 32000; 
        var myHand = new List<Tile>(MahjongManager.instance.playerHand);
        
        // --- 役満判定とゲーム終了 ---
        
        if (ShosushiChecker.IsShosushi(myHand))
        {
            Debug.Log("🎉 小四喜です！");
            scoreManager?.AddScore(YAKUMAN_SCORE);
            GameOver(); 
        }
        else if (TsuisoChecker.IsTsuiso(myHand))
        {
            Debug.Log("🎉 字一色です！");
            scoreManager?.AddScore(YAKUMAN_SCORE);
            GameOver(); 
        }
        else if (RyuuisoChecker.IsRyuuiso(myHand))
        {
            Debug.Log("🎉 緑一色です！");
            scoreManager?.AddScore(YAKUMAN_SCORE);
            GameOver(); 
        }
        else if (DaisushiChecker.IsDaisushi(myHand))
        {
            Debug.Log("🎉 大四喜です！");
            scoreManager?.AddScore(YAKUMAN_SCORE * 2);
            GameOver(); 
        }
        else if (ChinroutouChecker.IsChinroutou(myHand))
        {
            Debug.Log("🎉 清老頭です！");
            scoreManager?.AddScore(YAKUMAN_SCORE);
            GameOver(); 
        }
        else if (KokushiChecker.IsKokushi(myHand))
        {
            Debug.Log("🎉 国士無双です！");
            scoreManager?.AddScore(YAKUMAN_SCORE);
            GameOver(); 
        }
        else if (SuukantsuChecker.IsSuukantsu(myHand))
        {
            Debug.Log("🎉 四槓子です！");
            scoreManager?.AddScore(YAKUMAN_SCORE);
            GameOver(); 
        }
        else if (DaisangenChecker.IsDaisangen(myHand))
        {
            Debug.Log("🎉 大三元です！");
            scoreManager?.AddScore(YAKUMAN_SCORE);
            GameOver(); 
        }
        else if (SuuankouChecker.IsSuuankou(myHand))
        {
            Debug.Log("🎉 四暗刻です！");
            scoreManager?.AddScore(YAKUMAN_SCORE);
            GameOver(); 
        }
        else
        {
            Debug.Log("役満ではありません。（他の役の判定は未実装です）");
        }
        
        if (scoreManager == null)
        {
            Debug.LogError("Score Manager (Shooter2D) が設定されていません。Inspectorを確認してください！");
        }
    }

    // ----------------------------------------------------
    // ★★★ ゲーム終了メソッド (すべての要素を停止) ★★★
    // ----------------------------------------------------
    public void GameOver()
    {
        Debug.Log("役満成立！ゲーム終了！");

        // 1. プレイヤーの移動を停止
        if (playerMove != null)
        {
            playerMove.enabled = false;
        }

        // 2. BGMとSEの再生を停止
        if (raceBGM != null && raceBGM.isPlaying)
        {
            raceBGM.Stop();
        }
        if (countdownSE != null && countdownSE.isPlaying)
        {
            countdownSE.Stop();
        }

        // ★★★ 3. NPCの移動を停止 (NPCplayer.StopMovement()を呼び出す) ★★★
        if (npcMoveScripts != null)
        {
            foreach (var script in npcMoveScripts)
            {
                if (script != null)
                {
                    // NPCplayerスクリプトのStopMovementメソッドを呼び出し、Rigidbodyの速度をリセットさせる
                    script.StopMovement();
                }
            }
            Debug.Log($"NPC {npcMoveScripts.Length} 体の停止メソッドを呼び出しました。");
        }

        // 4. リザルト画面表示
        if (ResultPanel1 != null)
        {
            ResultPanel1.SetActive(true);
        }
        else
        {
            Debug.LogError("ResultPanel1が設定されていません。");
        }
    }
}