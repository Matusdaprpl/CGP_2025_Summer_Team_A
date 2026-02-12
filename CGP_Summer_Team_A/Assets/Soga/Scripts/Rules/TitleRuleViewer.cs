using UnityEngine;
using UnityEngine.UI; 

public class TitleRuleViewer : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject rulePanel;
    [SerializeField] private Image slideImage;    

    [SerializeField] private GameObject[] objectsToHide; 

    [Header("Slide Data")]
    [SerializeField] private Sprite[] rulePages;    

    [Header("Buttons")]
    [SerializeField] private Button nextButton;     
    [SerializeField] private Button backButton;     

    private int currentPageIndex = 0;

    void Start()
    {
        CloseRule();
    }

    public void OpenRule()
    {
        rulePanel.SetActive(true);
        
        if (objectsToHide != null)
        {
            foreach (GameObject obj in objectsToHide)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        ShowPage(0); 
    }

    public void CloseRule()
    {
        rulePanel.SetActive(false);

        if (objectsToHide != null)
        {
            foreach (GameObject obj in objectsToHide)
            {
                if (obj != null) obj.SetActive(true);
            }
        }
    }

    public void OnClickNext()
    {
        if (currentPageIndex < rulePages.Length - 1)
        {
            ShowPage(currentPageIndex + 1);
        }
    }

    public void OnClickBack()
    {
        if (currentPageIndex > 0)
        {
            ShowPage(currentPageIndex - 1);
        }
    }

    private void ShowPage(int index)
    {
        currentPageIndex = index;
        if (rulePages.Length > 0 && slideImage != null)
        {
            slideImage.sprite = rulePages[index];
        }

        if (backButton != null) backButton.interactable = (currentPageIndex > 0);
        if (nextButton != null) nextButton.interactable = (currentPageIndex < rulePages.Length - 1);
    }
}