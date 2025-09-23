using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; 

public class GameManager2 : MonoBehaviour
{
    public Button agariButton; 
    
    [Header("Debug Hand Selection (Set only ONE to true)")]
    public bool forceDaisangenHand = false; 
    public bool forceSuuankouHand = false; 
    public bool forceKokushiHand = false;
    public bool forceSuukantsuHand = false;

    void Start()
    {
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
    
    private void SetSuukantsuHand()
    {
        // 4つの4枚組のうち、和了牌を含めた14枚の牌を設定
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Pinzu, 1), (Suit.Pinzu, 1), (Suit.Pinzu, 1), (Suit.Pinzu, 1), 
            (Suit.Pinzu, 2), (Suit.Pinzu, 2), (Suit.Pinzu, 2), (Suit.Pinzu, 2), 
            (Suit.Pinzu, 3), (Suit.Pinzu, 3), (Suit.Pinzu, 3), (Suit.Pinzu, 3), 
            
            (Suit.Pinzu, 4), (Suit.Pinzu, 4) 
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
            (Suit.Honor, 1), (Suit.Honor, 1), (Suit.Honor, 1), 
            (Suit.Honor, 2), (Suit.Honor, 2) 
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
            (Suit.Honor, 1) 
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
        
        // 役満の判定を順番に行う（デバッグログを追加）
        
        Debug.Log("[Yakuman Check] Starting check...");
        if (KokushiChecker.IsKokushi(myHand))
        {
            Debug.Log("[Yakuman Check] Passed KokushiChecker.");
            Debug.Log("🎉 国士無双です！");
        }
        else if (SuukantsuChecker.IsSuukantsu(myHand))
        {
            Debug.Log("[Yakuman Check] Passed SuukantsuChecker.");
            Debug.Log("🎉 四槓子です！");
        }
        else if (DaisangenChecker.IsDaisangen(myHand))
        {
            Debug.Log("[Yakuman Check] Passed DaisangenChecker.");
            Debug.Log("🎉 大三元です！");
        }
        else if (SuuankouChecker.IsSuuankou(myHand))
        {
            Debug.Log("[Yakuman Check] Passed SuuankouChecker.");
            Debug.Log("🎉 四暗刻です！");
        }
        else
        {
            Debug.Log("[Yakuman Check] No Yakuman found.");
            Debug.Log("役満ではありません。（他の役の判定は未実装です）");
        }
    }
}