using UnityEngine;
using Common;

namespace EC.System
{
	public class DestoryOverTime : MonoBehaviourExtension
    {
        public float timer;
        void Start()
        {
            Destroy(gameObject, timer);
        }
    }
}
