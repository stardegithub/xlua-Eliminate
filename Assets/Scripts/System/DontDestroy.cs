using UnityEngine;

namespace EC.System
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
