using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private AudioClip m_title;
    [SerializeField] private AudioClip m_stage;
    [SerializeField] private Button m_button;

    void Awake()
    {
        m_button.onClick.AddListener(() => SceneController.Instance.ChangeScene(SceneType.StageScene));
        m_button.onClick.AddListener(() => SoundManager.Instance.ChangeBGM(m_stage));
    }

    private void Start() => SoundManager.Instance.FadeInBGM(m_title);
    public void OnClickExit() => Application.Quit();

}
