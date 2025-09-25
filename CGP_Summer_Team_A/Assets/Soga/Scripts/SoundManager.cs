using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource bgmSource;
    public AudioSource seSource;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //BGM
    public void PlayBGM(AudioClip bgmClip)
    {
        bgmSource.Stop();
        bgmSource.clip = bgmClip;
        bgmSource.Play();
    }

    //スタートのSE
    public void PlaySE(AudioClip seClip)
    {
        seSource.PlayOneShot(seClip);
    }
}
