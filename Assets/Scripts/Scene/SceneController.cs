using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[SerializeField]
public enum SceneType
{
    StartScene,
    StageScene
}

[RequireComponent(typeof(SceneTransition))]
public class SceneController : Singleton<SceneController>
{
    private SceneTransition m_transition;
    private WaitUntil m_untilTransition;

    public void ChangeScene(SceneType type, float outDelay = 2.5f, float inDelay = 2.5f) =>
        StartCoroutine(ChangeSceneCoroutine(type, outDelay, inDelay));

    public void RestartScene(float outDelay = 1.5f, float inDelay = 1.5f) =>
        StartCoroutine(ChangeSceneCoroutine(GetSceneType(), outDelay, inDelay));

    private void Awake()
    {
        m_transition = GetComponent<SceneTransition>();
        m_untilTransition = new WaitUntil(() => m_transition.IsDone);
    }

    private SceneType GetSceneType()
    {
        return (SceneType)Enum.Parse(typeof(SceneType), SceneManager.GetActiveScene().name);
    }

    private IEnumerator ChangeSceneCoroutine(SceneType type, float outDelay, float inDelay)
    {
        m_transition.FadeOut(outDelay);

        Pause.IsPause = true;
        var scene = SceneManager.LoadSceneAsync(type.ToString());
        scene.allowSceneActivation = false;
        do
        {
            yield return null;
        } while (scene.progress < 0.9f);

        yield return m_untilTransition;
        Pause.IsPause = false;
        scene.allowSceneActivation = true;

        m_transition.FadeIn(inDelay);
        yield return m_untilTransition;
    }
}
