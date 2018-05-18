using UnityEngine;

namespace GameSystem
{
    public class DontDestroy : MonoBehaviour
    {
        // Use this for initialization
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
