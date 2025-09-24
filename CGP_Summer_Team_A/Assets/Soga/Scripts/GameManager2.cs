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
    public bool forceDaisushiHand = false;
    public bool forceChinroutouHand = false;
    public bool forceRyuuisoHand = false;
    public bool forceTsuisoHand = false;
    public bool forceShosushiHand = false;

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

    private void SetDaisushiHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Honor, 1), (Suit.Honor, 1), (Suit.Honor, 1), // 東
            (Suit.Honor, 2), (Suit.Honor, 2), (Suit.Honor, 2), // 南
            (Suit.Honor, 3), (Suit.Honor, 3), (Suit.Honor, 3), // 西
            (Suit.Honor, 4), (Suit.Honor, 4), (Suit.Honor, 4), // 北
            
            (Suit.Pinzu, 1), (Suit.Pinzu, 1) // 雀頭
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
            
            (Suit.Souzu, 1), (Suit.Souzu, 1) // 雀頭
        };
        PassHandToManager(handData, "清老頭");
    }

    private void SetRyuuisoHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Souzu, 2), (Suit.Souzu, 2), (Suit.Souzu, 2),
            (Suit.Souzu, 3), (Suit.Souzu, 3), (Suit.Souzu, 3),
            (Suit.Souzu, 4), (Suit.Souzu, 4), (Suit.Souzu, 4),
            (Suit.Souzu, 6), (Suit.Souzu, 6), (Suit.Souzu, 6),
            
            (Suit.Honor, 6), (Suit.Honor, 6) // 發の雀頭
        };
        PassHandToManager(handData, "緑一色");
    }
    
    private void SetTsuisoHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Honor, 1), (Suit.Honor, 1), (Suit.Honor, 1), // 東 x 3
            (Suit.Honor, 2), (Suit.Honor, 2), (Suit.Honor, 2), // 南 x 3
            (Suit.Honor, 3), (Suit.Honor, 3), (Suit.Honor, 3), // 西 x 3
            (Suit.Honor, 4), (Suit.Honor, 4), (Suit.Honor, 4), // 北 x 3
            
            (Suit.Honor, 5), (Suit.Honor, 5) // 白 x 2
        };
        PassHandToManager(handData, "字一色");
    }

    private void SetShosushiHand()
    {
        var handData = new List<(Suit suit, int rank)>
        {
            (Suit.Honor, 1), (Suit.Honor, 1), (Suit.Honor, 1), // 東
            (Suit.Honor, 2), (Suit.Honor, 2), (Suit.Honor, 2), // 南
            (Suit.Honor, 3), (Suit.Honor, 3), (Suit.Honor, 3), // 西
            (Suit.Honor, 4), (Suit.Honor, 4), // 北（雀頭）
            
            (Suit.Manzu, 5), (Suit.Manzu, 6), (Suit.Manzu, 7) // 順子
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

        var myHand = new List<Tile>(MahjongManager.instance.playerHand);

        Debug.Log($"和了ボタン押下: 手牌 ={string.Join(", ", myHand.Select(t => t.GetDisplayName()))}") ;
        
        // 役満の判定を順番に行う
        
        Debug.Log("[Yakuman Check] Starting check...");
        
        if (ShosushiChecker.IsShosushi(myHand))
        {
            Debug.Log("[Yakuman Check] Passed ShosushiChecker.");
            Debug.Log("🎉 小四喜です！");
        }
        else if (TsuisoChecker.IsTsuiso(myHand))
        {
            Debug.Log("[Yakuman Check] Passed TsuisoChecker.");
            Debug.Log("🎉 字一色です！");
        }
        else if (RyuuisoChecker.IsRyuuiso(myHand))
        {
            Debug.Log("[Yakuman Check] Passed RyuuisoChecker.");
            Debug.Log("🎉 緑一色です！");
        }
        else if (DaisushiChecker.IsDaisushi(myHand))
        {
            Debug.Log("[Yakuman Check] Passed DaisushiChecker.");
            Debug.Log("🎉 大四喜です！");
        }
        else if (ChinroutouChecker.IsChinroutou(myHand))
        {
            Debug.Log("[Yakuman Check] Passed ChinroutouChecker.");
            Debug.Log("🎉 清老頭です！");
        }
        else if (KokushiChecker.IsKokushi(myHand))
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