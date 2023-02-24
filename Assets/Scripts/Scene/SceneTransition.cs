using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public bool IsDone { private set; get; } = false;

    private Image m_img;
    private IEnumerator m_curCoroutine;
    private bool m_isClicked = false;
    private Canvas m_canvas;

    private void Awake()
    {
        m_img = transform.GetChild(0).GetComponent<Image>();
        m_canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && m_canvas.enabled) m_isClicked = true;
    }

    public void FadeOut(float delay = 1.5f)
    {
        IsDone = false;
        m_canvas.enabled = true;
        m_curCoroutine = FadeOutCoroutine(delay);
        StartCoroutine(m_curCoroutine);
    }

    public void FadeIn(float delay = 1.5f)
    {
        IsDone = false;
        m_canvas.enabled = true;
        m_curCoroutine = FadeInCoroutine(delay);
        StartCoroutine(m_curCoroutine);
    }

    private IEnumerator FadeOutCoroutine(float delay)
    {
        float temp = 1f / delay;
        while (m_img.color.a < 0.99f && !m_isClicked)
        {
            m_img.color += new Color(0f, 0f, 0f, temp * Time.unscaledDeltaTime);
            yield return null;
        }

        m_isClicked = false;
        m_img.color = new Color(m_img.color.r, m_img.color.g, m_img.color.b, 1f);
        m_curCoroutine = null;
        IsDone = true;
    }

    private IEnumerator FadeInCoroutine(float delay)
    {
        float temp = 1f / delay;
        while (m_img.color.a > 0.01f && !m_isClicked)
        {
            m_img.color -= new Color(0f, 0f, 0f, temp * Time.unscaledDeltaTime);
            yield return null;
        }

        m_isClicked = false;
        m_img.color = new Color(m_img.color.r, m_img.color.g, m_img.color.b, 0f);
        m_curCoroutine = null;
        m_canvas.enabled = false;
        IsDone = true;
    }
}
