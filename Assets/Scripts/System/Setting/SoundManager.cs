using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType { Master, BGM, SE }

public class SoundManager : Singleton<SoundManager>
{
    [Header("오디오 믹서")]
    [SerializeField] private AudioMixer m_mainMixer;
    [SerializeField] private AudioSource m_bgmSource;
    [SerializeField] private AudioSource m_seSource;

    private IEnumerator m_bgmCoroutine;

    private readonly Dictionary<string, AudioClip> m_bgmDic = new();
    private bool m_isChanging = false;
    private bool m_skipFlag = false;

    private readonly Dictionary<string, AudioClip> m_seDic = new();

    void Start()
    {
        var option = SaveManager.Instance.LoadSystemOption();
        if (option is null)
        {
            option = new(0.5f, 1f);
        }

        SetVolume(SoundType.BGM, option.bgm);
        SetVolume(SoundType.SE, option.se);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && m_skipFlag) m_skipFlag = false;
    }

    public void PlayBGM(AudioClip clip, float volume = 1f)
    {
        if (clip is null) return;

        if (m_bgmCoroutine is not null) StopCoroutine(m_bgmCoroutine);
        if (m_bgmSource.isPlaying) m_bgmSource.Stop();

        m_bgmSource.volume = volume;
        m_bgmSource.clip = clip;
        m_bgmSource.Play();
    }

    public void PlaySE(AudioClip clip, float volume = 1f)
    {
        if (clip is null) return;
        m_seSource.PlayOneShot(clip, volume);
    }

    public void ChangeBGM(AudioClip clip, float volume = 1f, float outDelay = 2.5f, float inDelay = 2.5f)
    {
        if (clip is null) return;
        if (m_bgmCoroutine is not null) StopCoroutine(m_bgmCoroutine);
        m_bgmCoroutine = ChangeBGMCoroutine(clip, volume, outDelay, inDelay);
        StartCoroutine(m_bgmCoroutine);
    }

    public void FadeInBGM(AudioClip clip, float delay = 2.5f, float volume = 1f)
    {
        if (clip is null) return;
        if (m_bgmCoroutine is not null) StopCoroutine(m_bgmCoroutine);
        m_bgmCoroutine = FadeInBGMCoroutine(clip, delay, volume);
        StartCoroutine(m_bgmCoroutine);
    }

    public void FadeOutBGM(float delay = 2.5f)
    {
        if (m_bgmCoroutine is not null) StopCoroutine(m_bgmCoroutine);
        m_bgmCoroutine = FadeOutBGMCoroutine(delay);
        StartCoroutine(m_bgmCoroutine);
    }

    public void PauseBGM()
    {
        if (m_bgmSource.isPlaying) m_bgmSource.Pause();
    }

    public void UnPauseBGM()
    {
        if (!m_bgmSource.isPlaying) m_bgmSource.UnPause();
    }


    #region 볼륨 설정

    public void SetVolume(SoundType type, float volume)
    {
        if (volume < 0.001f) volume = 0.00000001f;
        m_mainMixer.SetFloat(type.ToString(), Mathf.Log10(volume) * 20);
    }

    public float GetVolume(SoundType type)
    {
        m_mainMixer.GetFloat(type.ToString(), out float result);
        return Mathf.Pow(10, result * 0.05f);
    }

    #endregion


    #region CSV 등 사용 시 어드레서블 에셋으로 가져왔을 때 사용

    public void AddBGM(string name, AudioClip clip)
    {
        if (clip is null) return;
        m_bgmDic[name] = clip;
    }

    public void AddSE(string name, AudioClip clip)
    {
        if (clip is null) return;
        m_seDic[name] = clip;
    }

    public void ClearAll()
    {
        m_bgmDic.Clear();
        m_seDic.Clear();
    }

    public void PlaySE(string name, float volume = 1f)
    {
        if (!m_seDic.TryGetValue(name, out AudioClip currentClip)) return;
        m_seSource.PlayOneShot(currentClip, volume);
    }

    public void PlayBGM(string name, float volume = 1f)
    {
        if (!m_bgmDic.TryGetValue(name, out AudioClip currentClip)) return;
        PlayBGM(currentClip, volume);
    }

    public void FadeInBGM(string name, float volume = 1f, float delay = 5f)
    {
        if (!m_bgmDic.TryGetValue(name, out AudioClip currentClip)) return;
        FadeInBGM(currentClip, volume, delay);
    }

    public void ChangeBGM(string name, float volume = 1f, float outDelay = 2.5f, float inDelay = 2.5f)
    {
        if (!m_bgmDic.TryGetValue(name, out AudioClip currentClip)) return;
        ChangeBGM(currentClip, volume, outDelay, inDelay);
    }

    #endregion


    #region 코루틴

    private IEnumerator FadeOutBGMCoroutine(float delay)
    {
        float temp = m_bgmSource.volume / delay;
        while (m_bgmSource.volume > 0.0001f)
        {
            m_bgmSource.volume -= Time.unscaledDeltaTime * temp;
            yield return null;
        }
        m_bgmSource.volume = 0f;
        m_isChanging = false;
    }

    private IEnumerator FadeInBGMCoroutine(AudioClip clip, float delay, float volume = 1f)
    {
        m_bgmSource.clip = clip;
        m_bgmSource.Play();

        float temp = volume / delay;
        while (m_bgmSource.volume < volume)
        {
            m_bgmSource.volume += Time.unscaledDeltaTime * temp;
            yield return null;
        }
        m_bgmSource.volume = volume;
        m_isChanging = false;
    }

    private IEnumerator ChangeBGMCoroutine(AudioClip clip, float volume, float outDelay, float inDelay)
    {
        if (m_bgmSource.clip is not null && m_bgmSource.volume > 0f)
        {
            m_isChanging = true;
            IEnumerator fadeout = FadeOutBGMCoroutine(outDelay);
            StartCoroutine(fadeout);
            m_skipFlag = true;
            yield return new WaitUntil(() => !m_isChanging || !m_skipFlag);
            if (fadeout is not null) StopCoroutine(fadeout);
            m_skipFlag = false;
        }

        m_isChanging = true;
        StartCoroutine(FadeInBGMCoroutine(clip, inDelay, volume));
        yield return new WaitUntil(() => !m_isChanging);
    }

    #endregion
}