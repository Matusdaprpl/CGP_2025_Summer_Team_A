using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; 
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using TMPro;

public class GameManager2 : MonoBehaviour
{
    public static int raceCount = 0;
    private const int RACE_LIMIT = 4; 

    [Header("01. ゲームコアと終了管理")]
    public PlayerMove playerMove;
    public AudioSource raceBGM;
    public AudioSource countdownSE;
    public AudioSource goalSE;

    [Tooltip("HierarchyのNPCオブジェクトにアタッチされているNPCplayerスクリプトをすべてここに設定します。")]
    public NPCplayer[] npcMoveScripts; 

    public Button agariButton; 
    public Shooter2D scoreManager; 

    private bool isGameStarted = false;

    [Header("テスト用配牌")]
    public bool forceDaisangenHand = false; 
    public bool forceKokushiHand = false;
    public bool forceDaisushiHand = false;
    public bool forceChinroutouHand = false;
    public bool forceRyuuisoHand = false;
    public bool forceTsuisoHand = false;
    public bool forceShosushiHand = false;
    public bool forceChuurenHand = false; 
    
    [Header("リザルト画面設定")]
    public GameObject ResultPanel; // 役満勝利時に表示するパネル
    public Image yakumanImage;
    public GameObject ResultPanel3; // (流局/NPCゴール用と仮定)
    public Image goalImage; 

    [Header("04. シーン遷移とリザルトボタン (要Inspector設定)")]
    public string titleSceneName = "TitleScene"; 
    public Button nextGameButton;
    public Button backToTitleButton;
    
    public Button nextGameButton2;     
    public Button backToTitleButton2;  
    
    [Header("05. スプライト設定")]
    [SerializeField] private Sprite kokushiSprite;
    [SerializeField] private Sprite daisangenSprite;
    [SerializeField] private Sprite daisushiSprite;
    [SerializeField] private Sprite chinroutouSprite;
    [SerializeField] private Sprite ryuuisoSprite;
    [SerializeField] private Sprite tsuisoSprite;
    [SerializeField] private Sprite shosushiSprite;
    [SerializeField] private Sprite chuurenSprite;
    [SerializeField] private Sprite goalSprite;
    [SerializeField] private Sprite doubleYakumanSprite;
    [SerializeField] private Sprite tripleYakumanSprite;

    [Header("レース情報UI")]
    public TMP_Text raceCountTMP;
    private bool raceCountHidden = false;

    [Header("Player表示")]
    public TMP_Text playerNameText;
    private bool playerNameHidden = false;


    [Header("ランキング用データ")]
    public static int playerFinalScore = 10000;
    public static List<(string name, int score)> npcFinalScores = new List<(string name, int score)>();

    public static Dictionary<string, int> npcPersistentScores = new Dictionary<string, int>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeRaceCount()
    {
        raceCount = 0;
        Shooter2D.score = 10000;
        npcPersistentScores.Clear();
    }
    void Start()
    {
        StartCoroutine(InitializeAfterFrames());
    }

    IEnumerator InitializeAfterFrames()
    {
        yield return null;

        raceCount++;
        Debug.Log($"現在のレース: {raceCount} / {RACE_LIMIT}");
        UpdateRaceCountUI(); 
        
        if (ResultPanel != null)
        {
            ResultPanel.SetActive(false); 
        }

        if (playerMove == null)
        {
            playerMove = FindFirstObjectByType<PlayerMove>();
        }
        if (raceBGM == null) raceBGM = GameObject.Find("RaceBGM")?.GetComponent<AudioSource>();
        if (countdownSE == null) countdownSE = GameObject.Find("CountdownSE")?.GetComponent<AudioSource>();
        if (goalSE == null) goalSE = GameObject.Find("GoalSE")?.GetComponent<AudioSource>();

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

        // ★★★ NPC勝利・流局用ボタンのリスナー設定 ★★★
        if (nextGameButton2 != null)
        {
            nextGameButton2.onClick.AddListener(OnNextGameButton);
            nextGameButton2.gameObject.SetActive(false); // 初期状態を非表示にする
        }
        if (backToTitleButton2 != null)
        {
            backToTitleButton2.onClick.AddListener(OnBackToTitleButton);
            backToTitleButton2.gameObject.SetActive(false); // 初期状態を非表示にする
        }
        
        // レース間で保持したNPCスコアを復元
        RestoreNpcScoresToNpcs();
        
        // --- テスト配牌ロジック ---
        if (forceDaisangenHand)
        {
            SetDaisangenHand();
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
            agariButton.interactable = false; // ★ 初期状態では無効化
            Debug.Log("和了ボタンが設定されました。");
        }
        else
        {
            Debug.LogError("和了ボタンが設定されていません。");
        }

        StartCoroutine(WaitCountdownAndHideRaceCount());
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
            
            (Suit.Pinzu, 2), (Suit.Pinzu, 3), (Suit.Pinzu, 4),
            (Suit.Souzu, 7), (Suit.Souzu, 7)
        };
        PassHandToManager(handData, "大三元");
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
            (Suit.Souzu, 2), (Suit.Souzu, 3), (Suit.Souzu, 4), 
            (Suit.Souzu, 6), (Suit.Souzu, 6), (Suit.Souzu, 6), 
            (Suit.Souzu, 8), (Suit.Souzu, 8), (Suit.Souzu, 8), 
            (Suit.Honor, 6), (Suit.Honor, 6), (Suit.Honor, 6), 
            (Suit.Souzu, 4), (Suit.Souzu, 4)
        };
        PassHandToManager(handData, "緑一色");
    }

    private void SetTsuisoHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Honor, 1), (Suit.Honor, 1), // 東
            (Suit.Honor, 2), (Suit.Honor, 2), // 南
            (Suit.Honor, 3), (Suit.Honor, 3), // 西
            (Suit.Honor, 4), (Suit.Honor, 4), // 北
            (Suit.Honor, 5), (Suit.Honor, 5), // 白
            (Suit.Honor, 6), (Suit.Honor, 6), // 發
            (Suit.Honor, 7), (Suit.Honor, 7)  // 中
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
            (Suit.Manzu, 9), (Suit.Manzu, 1) 
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
        // ★ カウントダウン中は処理を行わない
        if (!isGameStarted)
        {
            //Debug.Log("カウントダウン中は和了できません。");
            return;
        }

        if (MahjongManager.instance == null)
        {
            Debug.LogError("MahjongManager.instance が null です。");
            return;
        }

        const int SINGLE_YAKUMAN_SCORE = 32000;
        var myHand = new List<Tile>(MahjongManager.instance.playerHand);

        int yakumanMultiplier  = 0;
        Sprite spriteToShow = null;
        bool isDaisushi = false;

        // 役満判定ロジック
        if (TsuisoChecker.IsTsuiso(myHand))
        {
            yakumanMultiplier += 1; Debug.Log("🎉 字一色です！");  spriteToShow = tsuisoSprite;
        }
        if (RyuuisoChecker.IsRyuuiso(myHand))
        {
            yakumanMultiplier += 1; Debug.Log("🎉 緑一色です！");  spriteToShow = ryuuisoSprite;
        }
        if (ChinroutouChecker.IsChinroutou(myHand))
        {
            yakumanMultiplier += 1; Debug.Log("🎉 清老頭です！");  spriteToShow = chinroutouSprite;
        }
        if (KokushiChecker.IsKokushi(myHand))
        {
            yakumanMultiplier += 1; Debug.Log("🎉 国士無双です！");  spriteToShow = kokushiSprite;
        }
        if (DaisangenChecker.IsDaisangen(myHand))
        {
            yakumanMultiplier += 1; Debug.Log("🎉 大三元です！");  spriteToShow = daisangenSprite;
        }
        
        if (ChuurenChecker.IsChuuren(myHand)) 
        {
            yakumanMultiplier += 1; Debug.Log("🎉 九蓮宝燈です！");  spriteToShow = chuurenSprite;
        }
        if (DaisushiChecker.IsDaisushi(myHand))
        {
            yakumanMultiplier += 2; Debug.Log("🎉 大四喜です！");  spriteToShow = daisushiSprite;
            isDaisushi = true;
        }
        else if (ShosushiChecker.IsShosushi(myHand))
        {
            yakumanMultiplier += 1; Debug.Log("🎉 小四喜です！"); spriteToShow = shosushiSprite;
        }
        //役満ではない時
        if (yakumanMultiplier == 0)
        {
            Debug.Log("役満ではありません。（他の役の判定は未実装です）");
            return;
        }

        if (!isDaisushi)
        {
            if (yakumanMultiplier >= 3)
            {
                if (tripleYakumanSprite != null)
                {
                    spriteToShow = tripleYakumanSprite;
                }
            }
            else if (yakumanMultiplier == 2)
            {
                if (doubleYakumanSprite != null)
                {
                    spriteToShow = doubleYakumanSprite;
                }
            }
        }


        int totalScore = SINGLE_YAKUMAN_SCORE * yakumanMultiplier;
        Debug.Log($"役満合計: {yakumanMultiplier} 倍 / 合計得点: {totalScore}");
        scoreManager?.AddScore(totalScore);

        Debug.Log($"現在のレース: {raceCount} / {RACE_LIMIT}");
        
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

            if (raceCount >= RACE_LIMIT)
            {
                //タイトルへ戻るボタンを表示
                Debug.Log($"4レース終了。タイトルへ戻るボタンを表示します。");
                if (nextGameButton != null) nextGameButton.gameObject.SetActive(false);
                if (backToTitleButton != null) backToTitleButton.gameObject.SetActive(true);
            }
            else 
            {
                Debug.Log("次のゲームへ進むボタンを表示します。");
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

    //次のゲームに進む (点数引き継ぎ) ★★★
    public void OnNextGameButton()
    {
        if (ResultPanel != null)
        {
            ResultPanel.SetActive(false);
        }
        // 次レース開始前にNPCスコアを保存
        SaveNpcScoresAcrossRaces();

        Debug.Log($"次のゲームに進みます。現在のレース: {raceCount}");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnBackToTitleButton()  //ランキングに移行するように変更
    {
        // 最新のスコアを収集
        CollectFinalScores();
        
        Debug.Log($"ランキングへ移行します。Player最終スコア: {playerFinalScore}");

        SceneManager.LoadScene("Ranking");
    }
    
    //ゲーム終了メソッド群
    public void GameOver()
    {
        if (playerMove != null)
        {
            playerMove.enabled = false;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
           Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
           if(rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        if (raceBGM != null && raceBGM.isPlaying)
        {
            raceBGM.Stop();
        }
        if (countdownSE != null && countdownSE.isPlaying)
        {
            countdownSE.Stop();
        }
        if (npcMoveScripts != null && npcMoveScripts.Length > 0)
        {
            foreach (var script in npcMoveScripts)
            {
                if (script != null)
                {
                    script.StopMovement();
                }
            }
        }
        else
        {
            Debug.LogWarning("NPCの移動スクリプトが設定されていません。");
        }

        // 結果中の射撃を無効化（PlayerのShooter2D）
        if (scoreManager != null)
        {
            scoreManager.enabled = false;
        }

        // 既に飛んでいる弾を掃除
        foreach (var bullet in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            Destroy(bullet);
        }
    }

    public void OnNpcWinResult()
    {
        Debug.Log($"NPC勝利。現在のレース: {raceCount} / {RACE_LIMIT}");

        if (raceCount >= RACE_LIMIT)
        {
            // タイトルへ戻るボタンを表示
            Debug.Log($"{RACE_LIMIT}レース終了。タイトルへ戻るボタンを表示します。");
            if (nextGameButton2 != null) nextGameButton2.gameObject.SetActive(false);
            if (backToTitleButton2 != null) backToTitleButton2.gameObject.SetActive(true);
        }
        else 
        {
            // 次のゲームへ進むボタンを表示
            Debug.Log("次のゲームへ進むボタンを表示します。");
            if (nextGameButton2 != null) nextGameButton2.gameObject.SetActive(true);
            if (backToTitleButton2 != null) backToTitleButton2.gameObject.SetActive(false);
        }
    }

    public void OnGoalResult()
    {
        Debug.Log($"流局。現在のレース: {raceCount} / {RACE_LIMIT}");

        if (raceCount >= RACE_LIMIT)
        {
            // タイトルへ戻るボタンを表示
            Debug.Log($"{RACE_LIMIT}レース終了。タイトルへ戻るボタンを表示します。");
            if (nextGameButton2 != null) nextGameButton2.gameObject.SetActive(false);
            if (backToTitleButton2 != null) backToTitleButton2.gameObject.SetActive(true);
        }
        else 
        {
            // 次のゲームへ進むボタンを表示
            Debug.Log("次のゲームへ進むボタンを表示します。");
            if (nextGameButton2 != null) nextGameButton2.gameObject.SetActive(true);
            if (backToTitleButton2 != null) backToTitleButton2.gameObject.SetActive(false);
        }
    }

    public void OnGoal(string characterName)
    {
        if (MahjongManager.instance.roundOver) return;
        MahjongManager.instance.roundOver = true;

        Debug.Log($"{characterName}がゴールしました。流局です。");

        if (goalSE != null)
        {
            goalSE.Play();
        }

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
        OnGoalResult(); 
    }

    private void UpdateRaceCountUI()
    {
        if (raceCountTMP != null)
        {
            raceCountTMP.text = $"レース: {raceCount} / {RACE_LIMIT}";
            raceCountTMP.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("raceCountのTextが設定されていません。");
        }
    }

    private void HideRaceCountUI()
    {
        if (raceCountHidden) return;
        raceCountHidden = true;
        if (raceCountTMP != null)
        {
            raceCountTMP.gameObject.SetActive(false);
        }
    }

    private void HidePlayerNameUI()
    {
        if (playerNameHidden) return;
        playerNameHidden = true;
        if (playerNameText != null)
        {
            playerNameText.gameObject.SetActive(false);
        }
    }

    private IEnumerator WaitCountdownAndHideRaceCount()
    {
        while (playerMove == null)
        {
            yield return null;
            playerMove = FindFirstObjectByType<PlayerMove>();
        }

        while (playerMove != null && playerMove.IsCountdownActive)
        {
            yield return null;
        }

        isGameStarted = true;
        if (agariButton != null)
        {
            agariButton.interactable = true;
            //Debug.Log("カウントダウン終了。和了ボタンを有効化しました。");
        }

        HideRaceCountUI();
        HidePlayerNameUI();
    }

    public static void SetNpcScore(string name, int score)
    {
        if (npcPersistentScores.ContainsKey(name))
        {
            npcPersistentScores[name] = score;
        }
        else
        {
            npcPersistentScores.Add(name, score);
        }
        // Debug.Log($"GameManager: NPCスコア更新 {name} = {score}");
    }
    private void SaveNpcScoresAcrossRaces()
    {
        if (npcMoveScripts == null) return;
        foreach (var npc in npcMoveScripts)
        {
            if (npc == null) continue;
            string name = npc.gameObject.name;
            int score = npc.score();
            SetNpcScore(name, score);        
        }
    }

    private void RestoreNpcScoresToNpcs()
    {
        if (npcMoveScripts == null) return;
        foreach (var npc in npcMoveScripts)
        {
            if (npc == null) continue;
            string name = npc.gameObject.name;
            if (npcPersistentScores.TryGetValue(name, out int saved))
            {
                npc.SetScore(saved);
            }
            else
            {
                npc.SetScore(10000); 
                SetNpcScore(name, 10000);
            }
        }
    }

    public static void ClearNpcPersistentScores()
    {
        npcPersistentScores.Clear();
        Debug.Log("NPCスコア永続辞書をクリアしました。");
    }

    private void CollectFinalScores()
    {
        // Playerのスコア
        playerFinalScore = Shooter2D.score;

        // NPCのスコア
        npcFinalScores.Clear();
        if (npcMoveScripts != null)
        {
            foreach (var npc in npcMoveScripts)
            {
                if (npc != null)
                {
                    int actualScore = npc.score();
                    npcFinalScores.Add((npc.gameObject.name, actualScore));
                    Debug.Log($"NPCスコア: {npc.gameObject.name} = {actualScore}点");
                }
            }
        }
        
        Debug.Log($"最終スコア - Player: {playerFinalScore}点");
    }

}