using UnityEngine;

namespace DevCase.Utility
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool m_ShuttingDown = false;
        private static object m_Lock = new object();
        private static T m_Instance;
        public static T Instance
        {
            get
            {
                lock (m_Lock)
                {
                    if (m_Instance == null || m_ShuttingDown)
                    {
                        m_Instance = (T)FindFirstObjectByType(typeof(T));
                        if (m_Instance == null)
                        {
                            var singletonObject = new GameObject();
                            m_Instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";
                            m_ShuttingDown = false;
                        }
                    }

                    return m_Instance;
                }
            }
        }
        private void OnApplicationQuit()
        {
            m_ShuttingDown = true;
        }

        private void OnDestroy()
        {
            m_ShuttingDown = true;
        }
    }
}

