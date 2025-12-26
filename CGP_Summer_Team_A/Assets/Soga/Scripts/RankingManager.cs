using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class RankingManager : MonoBehaviour
{
    [Header("設定")]
    public Transform rankingContent;
    public GameObject rankingItemPrefab; 
    public Button backToTitleButton;    
    public string titleSceneName = "TitleScene";

    void Start()
    {
        DisplayRanking();
        if (backToTitleButton != null)
        {backToTitleButton.onClick.AddListener(() => 
            {
                GameManager2.ClearNpcPersistentScores();
                
                GameManager2.raceCount = 0;
                Shooter2D.score = 10000;
                
                SceneManager.LoadScene(titleSceneName);
            });
        }
    }

    void DisplayRanking()
    {
        // データの統合
        var allScores = new List<(string name, int score)>();
        allScores.Add(("Player", GameManager2.playerFinalScore));
        allScores.AddRange(GameManager2.npcFinalScores);

        // スコア順に並び替え
        var sorted = allScores.OrderByDescending(x => x.score).ToList();

        // 既存の表示をクリア
        foreach (Transform child in rankingContent) Destroy(child.gameObject);

        // 生成
        for (int i = 0; i < sorted.Count; i++)
        {
            GameObject item = Instantiate(rankingItemPrefab, rankingContent);
            
            // テキストの流し込み（名前で検索）
            var rText = item.transform.Find("RankText").GetComponent<TextMeshProUGUI>();
            var nText = item.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            var sText = item.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();

            rText.text = $"{i + 1}位";
            nText.text = sorted[i].name;
            sText.text = $"{sorted[i].score}点";

            // 順位に応じた色分け
            Color rankColor = i switch
            {
                0 => new Color(1f, 0.84f, 0f),      // 金色
                1 => new Color(0.75f, 0.75f, 0.75f), // 銀色
                2 => new Color(0.8f, 0.5f, 0.2f),    // 銅色
                _ => Color.white
            };

            rText.color = rankColor;
            nText.color = Color.black;
            sText.color = Color.blue;
        }
    }
}