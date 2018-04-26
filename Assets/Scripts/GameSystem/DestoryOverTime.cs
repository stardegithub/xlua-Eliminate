using UnityEngine;

namespace GameSystem
{
    public class DestoryOverTime : MonoBehaviour
    {
        public float timer;
        void Start()
        {
            Destroy(gameObject, timer);
        }
    }
}
