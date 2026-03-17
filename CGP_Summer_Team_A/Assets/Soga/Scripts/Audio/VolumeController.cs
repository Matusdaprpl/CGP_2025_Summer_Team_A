using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private TextMeshProUGUI BGMText;

    [SerializeField] private Slider SESlider;
    [SerializeField] private TextMeshProUGUI SEText;    

    private const string BGMVolumeParameter = "BGMVolume";
    private const string SEVolumeParameter = "SEVolume";

    private void Start()
    {
        float savedBGMVolume = PlayerPrefs.GetFloat(BGMVolumeParameter, 1f);

        if(BGMSlider != null)
        {
            BGMSlider.value = savedBGMVolume;
            BGMSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        SetBGMVolume(savedBGMVolume);

        float savedSEVolume = PlayerPrefs.GetFloat(SEVolumeParameter, 1f);

        if(SESlider != null)
        {
            SESlider.value = savedSEVolume;
            SESlider.onValueChanged.AddListener(SetSEVolume);
        }

        SetSEVolume(savedSEVolume);
    }

    private void SetBGMVolume(float volume)
    {
        float db = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat(BGMVolumeParameter, db);

        if(BGMText != null)
        {
            BGMText.text = Mathf.RoundToInt(volume * 100f).ToString() + "%";
        }
        
        PlayerPrefs.SetFloat(BGMVolumeParameter, volume);
        PlayerPrefs.Save();
    }

    private void SetSEVolume(float volume)
    {
        float db = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat(SEVolumeParameter, db);

        if(SEText != null)
        {
            SEText.text = Mathf.RoundToInt(volume * 100f).ToString() + "%";
        }
        
        PlayerPrefs.SetFloat(SEVolumeParameter, volume);
        PlayerPrefs.Save();
    }
}
