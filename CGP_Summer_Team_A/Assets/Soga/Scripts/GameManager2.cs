using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameManager2 : MonoBehaviour // ← 'public'はここで使用
{
    public Button agariButton; 
    
    // ▼ 【追加】テスト用のフラグ
    [Header("Debug Hand Selection (Set only ONE to true)")]
    public bool forceDaisangenHand = true; 
    public bool forceSuuankouHand = false; 
    public bool forceKokushiHand = false;
    public bool forceSuukantsuHand = false; // 四槓子のテスト用
    
    // -------------------------------------------------------------------------
    // Start() メソッド
    // -------------------------------------------------------------------------
    void Start()
    {
        // 1. テスト配牌の実行
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
        
        // 2. ボタンリスナーの設定
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
    
    // -------------------------------------------------------------------------
    // 【テスト用配牌ロジック】
    // -------------------------------------------------------------------------

    private void SetDaisangenHand()
    {
        // SuitとRankは、MahjongManagerで定義されたものを使用
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Honor, 5), (Suit.Honor, 5), (Suit.Honor, 5), // 白
            (Suit.Honor, 6), (Suit.Honor, 6), (Suit.Honor, 6), // 發
            (Suit.Honor, 7), (Suit.Honor, 7), (Suit.Honor, 7), // 中
            
            (Suit.Pinzu, 2), (Suit.Pinzu, 2), (Suit.Pinzu, 2), 
            
            (Suit.Pinzu, 4), (Suit.Pinzu, 4) // 雀頭
        };
        PassHandToManager(handData, "大三元");
    }
    
    private void SetSuukantsuHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Manzu, 1), (Suit.Manzu, 1), (Suit.Manzu, 1), (Suit.Manzu, 1), // 槓子1
            (Suit.Manzu, 2), (Suit.Manzu, 2), (Suit.Manzu, 2), (Suit.Manzu, 2), // 槓子2
            (Suit.Pinzu, 1), (Suit.Pinzu, 1), (Suit.Pinzu, 1), (Suit.Pinzu, 1), // 槓子3
            
            (Suit.Pinzu, 2), (Suit.Pinzu, 2), // 雀頭
        };
        PassHandToManager(handData, "四槓子");
    }

    private void SetSuuankouHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Manzu, 1), (Suit.Manzu, 1), (Suit.Manzu, 1),
            (Suit.Manzu, 2), (Suit.Manzu, 2), (Suit.Manzu, 2),
            (Suit.Manzu, 3), (Suit.Manzu, 3), (Suit.Manzu, 3),
            (Suit.Honor, 1), (Suit.Honor, 1), (Suit.Honor, 1), // 東 
            
            (Suit.Honor, 2), (Suit.Honor, 2) // 南の雀頭
        };
        PassHandToManager(handData, "四暗刻");
    }
    
    private void SetKokushiHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Manzu, 1), (Suit.Manzu, 9), (Suit.Pinzu, 1), (Suit.Pinzu, 9), 
            (Suit.Souzu, 1), (Suit.Souzu, 9),
            (Suit.Honor, 1), (Suit.Honor, 2), (Suit.Honor, 3), (Suit.Honor, 4), 
            (Suit.Honor, 5), (Suit.Honor, 6), (Suit.Honor, 7), 
            
            (Suit.Honor, 1) // 東を雀頭にする
        };
        PassHandToManager(handData, "国士無双");
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

    // -------------------------------------------------------------------------
    // OnAgariButton() ロジック (判定順序修正済)
    // -------------------------------------------------------------------------
    
    public void OnAgariButton()
    {
        Debug.Log("和了ボタンが押されました。");
        if (MahjongManager.instance == null)
        {
            Debug.LogError("MahjongManager.instance が null です。");
            return;
        }

        var myHand = new List<Tile>(MahjongManager.instance.playerHand);

        Debug.Log($"和了ボタン押下: 手牌 ={string.Join(", ", myHand.Select(t => t.GetDisplayName()))}") ;

        // 役満の判定を順番に行う
        if (KokushiChecker.IsKokushi(myHand))
        {
            Debug.Log("🎉 国士無双です！");
        }
        
        // 2. 槓子形
        else if (SuukantsuChecker.IsSuukantsu(myHand)) 
        {
            Debug.Log("🎉 四槓子です！");
        }

        // 3. 字牌の組み合わせ
        else if (DaisangenChecker.IsDaisangen(myHand))
        {
            Debug.Log("🎉 大三元です！");
        }

        // 4. 和了の形（刻子形）
        else if (SuuankouChecker.IsSuuankou(myHand))
        {
            Debug.Log("🎉 四暗刻です！");
        }
        
        else
          
        else if (NinegateChecker.IsNineGates(myHand))
        {
            Debug.Log("🎉 九蓮宝燈です！");
        }
        else
                {
                    Debug.Log("役満ではありません。（他の役の判定は未実装です）");
                }
    }
}