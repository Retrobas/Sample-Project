using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Slider m_bgmSlider;
    [SerializeField] private Slider m_seSider;
    [SerializeField] private Button m_exitButton;

    private SystemOption option;

    void Start()
    {
        option = new(SoundManager.Instance.GetVolume(SoundType.BGM),
            SoundManager.Instance.GetVolume(SoundType.SE));

        m_bgmSlider.value = option.bgm;
        m_seSider.value = option.se;

        m_bgmSlider.onValueChanged.AddListener(delegate (float value) {
            SoundManager.Instance.SetVolume(SoundType.BGM, value);
            option.bgm = value;
        });

        m_seSider.onValueChanged.AddListener(delegate (float value) {
            SoundManager.Instance.SetVolume(SoundType.SE, value);
            option.se = value;
        });

        m_exitButton.onClick.AddListener(() => SaveManager.Instance.SaveSystemOption(option));
    }
}
