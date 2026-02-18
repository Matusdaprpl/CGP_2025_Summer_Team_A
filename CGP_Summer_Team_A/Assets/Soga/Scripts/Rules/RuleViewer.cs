using UnityEngine;

public class RuleViewer : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject rulePanel;

    [Header("Pause Settings")]
    [SerializeField] private bool pauseGame = true;

    private float previousTimeScale = 1f;

    void Start()
    {
        if (rulePanel != null) 
        {
            rulePanel.SetActive(false);
        }
    }

    public void OpenRule()
    {
        //Debug.Log("ボタンが押されました！");
        
        if (rulePanel == null) return;
        
        rulePanel.SetActive(true);
        Pause();
    }

    public void CloseRule()
    {
        if (rulePanel == null) return;

        rulePanel.SetActive(false);
        Resume();
    }

    private void Pause()
    {
        if (!pauseGame) return;

        if (Time.timeScale > 0f)
        {
            previousTimeScale = Time.timeScale;
        }
        
        Time.timeScale = 0f;
    }

    private void Resume()
    {
        if (!pauseGame) return;
        
        Time.timeScale = previousTimeScale;
    }

    private void OnDisable()
    {
        if (pauseGame && Time.timeScale == 0f)
        {
            Time.timeScale = previousTimeScale;
        }
    }
}