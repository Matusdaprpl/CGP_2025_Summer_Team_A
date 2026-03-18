using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class RuleViewer : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject rulePanel;
    [SerializeField] private GameObject audioPanel;

    [Header("Pause Settings")]
    [SerializeField] private bool pauseGame = true;
    private float previousTimeScale = 1f;
    private bool pauseApplied = false;

    [Header("Audio Settings")]
    [SerializeField] private bool pauseAudioOnRuleOpen = true;
    private bool previousAudioListenerPause = false;
    private bool audioPauseApplied = false;

    [Header("Button")]
    [SerializeField] private Button audioButton;

    [Header("SE Settings")]
    [SerializeField] private bool pauseSEOnRuleOpen = true;
    [SerializeField] private AudioMixer sfxMixer;               // SE用Mixer
    [SerializeField] private string sfxVolumeParameter = "SFXVolume"; // Exposed Parameter名
    [SerializeField] private float mutedDb = -80f;

    private float previousSfxDb = 0f;
    private bool sePauseApplied = false;
    public bool IsRulePanelOpen => rulePanel != null && rulePanel.activeSelf;
    public bool IsAnyPanelOpen =>
        (rulePanel != null && rulePanel.activeSelf) ||
        (audioPanel != null && audioPanel.activeSelf);


    void Start()
    {
        if (rulePanel != null) rulePanel.SetActive(false);
        if (audioPanel != null) audioPanel.SetActive(false);

        if (audioButton != null)
            audioButton.onClick.AddListener(ToggleAudioPanel);
        else
            Debug.LogWarning("[RuleViewer] audioButton が未設定です。");

        RefreshPauseState();
    }

    private void OnDestroy()
    {
        if (audioButton != null)
            audioButton.onClick.RemoveListener(ToggleAudioPanel);
    }

    private void OnDisable()
    {
        // 非アクティブ化時に停止状態を残さない
        if (pauseApplied && pauseGame)
        {
            Time.timeScale = previousTimeScale;
            pauseApplied = false;
        }

        if(audioPauseApplied && pauseAudioOnRuleOpen)
        {
            AudioListener.pause = previousAudioListenerPause;
            audioPauseApplied = false;
        }

        if (sePauseApplied && pauseSEOnRuleOpen && sfxMixer != null)
        {
            sfxMixer.SetFloat(sfxVolumeParameter, previousSfxDb);
            sePauseApplied = false;
        }
    }

    public void OpenRule()
    {
        if (rulePanel == null) return;
        rulePanel.SetActive(true);
        RefreshPauseState();
    }

    public void CloseRule()
    {
        if (rulePanel == null) return;
        rulePanel.SetActive(false);
        RefreshPauseState();
    }

    public void OpenAudioPanel()
    {
        if (audioPanel == null) return;
        audioPanel.SetActive(true);
        RefreshPauseState();
    }

    public void CloseAudioPanel()
    {
        if (audioPanel == null) return;
        audioPanel.SetActive(false);
        RefreshPauseState();
    }

    public void ToggleAudioPanel()
    {
        if (audioPanel == null) return;

        if (audioPanel.activeSelf) CloseAudioPanel();
        else OpenAudioPanel();
    }

    private void RefreshPauseState()
    {
        if (pauseGame)
        {
            bool anyPanelOpen =
                (rulePanel != null && rulePanel.activeSelf) ||
                (audioPanel != null && audioPanel.activeSelf);

            if (anyPanelOpen)
            {
                if (!pauseApplied)
                {
                    if (Time.timeScale > 0f) previousTimeScale = Time.timeScale;
                    Time.timeScale = 0f;
                    pauseApplied = true;
                }
            }
            else
            {
                if (pauseApplied)
                {
                    Time.timeScale = previousTimeScale;
                    pauseApplied = false;
                }
            }
        }

        RefreshAudioPauseState();
        RefreshSEPauseState();
    }

    private void RefreshAudioPauseState()
    {
        if(!pauseAudioOnRuleOpen) return;

        bool ruleOpen = (rulePanel != null && rulePanel.activeSelf);

        if(ruleOpen)
        {
            if(!audioPauseApplied)
            {
                previousAudioListenerPause = AudioListener.pause;
                AudioListener.pause = true;
                audioPauseApplied = true;
            }
        }
        else
        {
            if(audioPauseApplied)
            {
                AudioListener.pause = previousAudioListenerPause;
                audioPauseApplied = false;
            }
        }
    }

    private void RefreshSEPauseState()
    {
        if (!pauseSEOnRuleOpen || sfxMixer == null) return;

        bool ruleOpen = (rulePanel != null && rulePanel.activeSelf);

        if (ruleOpen)
        {
            if (!sePauseApplied)
            {
                sfxMixer.GetFloat(sfxVolumeParameter, out previousSfxDb);
                sfxMixer.SetFloat(sfxVolumeParameter, mutedDb);
                sePauseApplied = true;
            }
        }
        else
        {
            if (sePauseApplied)
            {
                sfxMixer.SetFloat(sfxVolumeParameter, previousSfxDb);
                sePauseApplied = false;
            }
        }
    }
}