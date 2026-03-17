using UnityEngine;
using UnityEngine.UI;

public class AudioPanelViewer : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject audioPanel;

    [SerializeField] private GameObject[] objectsToHide;

    [Header("Button")]
    [SerializeField] private Button audioButton;
    [SerializeField] private Button closeButton;

    void Start()
    {
        CloseAudioPanel();

        if (audioButton != null)
        {
            audioButton.onClick.AddListener(ToggleAudioPanel);
        }

        if(closeButton != null)
        {
            closeButton.onClick.AddListener(CloseAudioPanel);
        }
    }

    void ToggleAudioPanel()
    {
        if (audioPanel == null) return;

        if (audioPanel.activeSelf) CloseAudioPanel();
        else OpenAudioPanel();

    }

    void OpenAudioPanel()
    {
        if (audioPanel == null) return;
        audioPanel.SetActive(true);

        if (objectsToHide != null)
        {
            foreach (GameObject obj in objectsToHide)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

    }

    void CloseAudioPanel()
    {
        if (audioPanel == null) return;
        audioPanel.SetActive(false);

        if (objectsToHide != null)
        {
            foreach (GameObject obj in objectsToHide)
            {
                if (obj != null) obj.SetActive(true);
            }
        }
    }
}
