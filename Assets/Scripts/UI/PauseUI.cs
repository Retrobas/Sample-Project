using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public void OnClickTitle() 
    {
        SceneController.Instance.ChangeScene(SceneType.StartScene);
        SoundManager.Instance.FadeOutBGM();
    }

    public void OnClickExit() => Application.Quit();

    public void OnClickResume() => Pause.IsPause = false;
}
