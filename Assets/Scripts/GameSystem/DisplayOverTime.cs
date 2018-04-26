using UnityEngine;
using UnityEngine.UI;

namespace GameSystem
{
    public class DisplayOverTime : MonoBehaviour
    {

        public enum SetActiveType
        {
            Open,
            Close,
            PingPong,
        }

        public SetActiveType setActiveType;

        public Graphic graphic;

        public float interval = 1;

        private float cumulative = 0;


        void Start()
        {
            if (graphic == null)
            {
                graphic = GetComponent<Graphic>();
            }

            if (graphic == null)
            {
                this.enabled = false;
            }
        }

        void Update()
        {
            if (graphic == null)
            {
                return;
            }

            if (cumulative > interval)
            {
                cumulative = 0;

                switch (setActiveType)
                {
                    case SetActiveType.Open:
                        {
                            graphic.enabled = true;
                            return;
                        }
                    case SetActiveType.Close:
                        {
                            graphic.enabled = false;
                            return;
                        }
                    case SetActiveType.PingPong:
                        {
                            graphic.enabled = !graphic.enabled;
                            return;
                        }
                    default: return;
                }
            }

            cumulative += Time.deltaTime;
        }
    }
}
