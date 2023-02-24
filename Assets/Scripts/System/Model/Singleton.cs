using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly object m_lockObject = new();
    private static T m_instance;

    public static T Instance
    {
        get
        {
            lock (m_lockObject)
            {
                if (m_instance is null)
                {
                    m_instance = (T)FindObjectOfType(typeof(T));
                    if (m_instance is null)
                    {
                        m_instance = Instantiate(Resources.Load<T>("Singleton/" + typeof(T)));
                        DontDestroyOnLoad(m_instance.gameObject);
                    }
                }
                return m_instance;
            }
        }
    }
}
