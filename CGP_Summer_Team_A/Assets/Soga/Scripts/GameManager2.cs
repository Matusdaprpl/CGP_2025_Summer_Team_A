using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameManager2 : MonoBehaviour
{
    public Button agariButton; // 「和了」ボタン


    void Start()
    {
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
    public void OnAgariButton()
    {
        Debug.Log("和了ボタンが押されました。");
        if (MahjongManager.instance == null)
        {
            Debug.LogError("MahjongManager.instance が null です。");
            return;
        }

        // MahjongManager の手牌をコピー
        var myHand = new List<Tile>(MahjongManager.instance.playerHand);

        Debug.Log($"和了ボタン押下: 手牌 ={string.Join(", ", myHand.Select(t => t.GetDisplayName()))}") ;

        // 役満の判定を順番に行う
        if (KokushiChecker.IsKokushi(myHand))
        {
            Debug.Log("🎉 国士無双です！");
        }
        else if (SuuankouChecker.IsSuuankou(myHand))
        {
            Debug.Log("🎉 四暗刻です！");
        }
        else if (DaisangenChecker.IsDaisangen(myHand))
        {
            Debug.Log("🎉 大三元です！");
        }
        else
        {
            Debug.Log("役満ではありません。（他の役の判定は未実装です）");
        }
    }
}