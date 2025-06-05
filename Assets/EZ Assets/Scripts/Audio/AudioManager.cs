using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public List<AudioClip> sfxClips;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private Dictionary<string, AudioClip> sfxDict;

    void Awake()
    {
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Khởi tạo Dictionary cho SFX
        sfxDict = new Dictionary<string, AudioClip>();
        if (sfxClips != null)
        {
            foreach (var clip in sfxClips)
            {
                if (clip != null && !sfxDict.ContainsKey(clip.name))
                {
                    sfxDict.Add(clip.name, clip);
                }
                else if (clip == null)
                {
                    Debug.LogWarning("AudioManager: Found a null clip in sfxClips list!");
                }
            }
        }
        else
        {
            Debug.LogWarning("AudioManager: sfxClips list is null!");
        }
    }

    void Start()
    {
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        PlayMusic(backgroundMusic);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null)
        {
            Debug.LogWarning("AudioManager: Cannot play music. MusicSource or clip is null!");
            return;
        }
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null)
        {
            Debug.LogWarning("AudioManager: MusicSource is null!");
            return;
        }
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        if (musicSource == null)
        {
            Debug.LogWarning("AudioManager: MusicSource is null!");
            return;
        }
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource == null)
        {
            Debug.LogWarning("AudioManager: MusicSource is null!");
            return;
        }
        musicSource.UnPause();
    }

    public void PlaySFX(string clipName)
    {
        if (sfxSource == null)
        {
            Debug.LogWarning("AudioManager: SFXSource is null!");
            return;
        }
        if (!sfxDict.ContainsKey(clipName))
        {
            Debug.LogWarning($"AudioManager: No SFX clip found for name: {clipName}");
            return;
        }
        sfxSource.PlayOneShot(sfxDict[clipName], sfxVolume);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
        {
            Debug.LogWarning("AudioManager: Cannot play SFX. SFXSource or clip is null!");
            return;
        }
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        // Lưu ý: PlayOneShot không bị ảnh hưởng bởi sfxSource.volume,
        // nhưng có thể cập nhật để nhất quán
    }

    // Thêm hiệu ứng fade cho nhạc
    public void FadeMusic(float targetVolume, float duration)
    {
        if (musicSource == null)
        {
            Debug.LogWarning("AudioManager: MusicSource is null!");
            return;
        }
        StartCoroutine(FadeMusicCoroutine(targetVolume, duration));
    }

    private System.Collections.IEnumerator FadeMusicCoroutine(float targetVolume, float duration)
    {
        float startVolume = musicSource.volume;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }
        musicSource.volume = targetVolume;
        musicVolume = targetVolume;
    }
}