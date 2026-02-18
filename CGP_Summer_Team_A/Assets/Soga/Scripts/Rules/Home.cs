using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    [SerializeField] private Button homeButton;

    void Start()
    {
        if (homeButton != null)
        {
            homeButton.onClick.AddListener(OnHomeButtonClicked);
        }
        else
        {
            Debug.LogWarning("Home.cs: homeButtonが設定されていません。");
        }
    }

    private void OnHomeButtonClicked()
    {
        Debug.Log("ホームボタンが押されました。タイトルシーンに遷移します。");
        
        // スコアとレース情報をリセット
        GameManager2.ClearNpcPersistentScores();
        GameManager2.raceCount = 0;
        Shooter2D.score = 10000;
        
        SceneManager.LoadScene("Title");
    }
}
