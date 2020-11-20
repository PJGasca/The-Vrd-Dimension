using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerBeam : MonoBehaviour
    {
        public HashSet<GameObject> ObjectsInBeam { get; private set; } = new HashSet<GameObject>();

        public enum BeamMode { ATTRACT, REPEL, BOTH, OFF };

        [SerializeField]
        private Color offColor;

        [SerializeField]
        private Color attractingColor;

        [SerializeField]
        private Color repellingColor;

        [SerializeField]
        private Color bothColor;

        private BeamMode mode = BeamMode.OFF;

        private LineRenderer lineRenderer;

        public BeamMode Mode
        {
            get
            {
                return mode;
            }

            set
            {
                mode = value;

                switch(mode)
                {
                    case BeamMode.ATTRACT:
                        SetBeamColor(attractingColor);
                        break;

                    case BeamMode.REPEL:
                        SetBeamColor(repellingColor);
                        break;

                    case BeamMode.BOTH:
                        SetBeamColor(bothColor);
                        break;

                    default:
                        SetBeamColor(offColor);
                        break;
                }
            }
        }

        public void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnEnable()
        {
            SetBeamColor(offColor);
        }

        private void OnTriggerEnter(Collider other)
        {
            ObjectsInBeam.Add(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            ObjectsInBeam.Remove(other.gameObject);
        }

        private void SetBeamColor(Color color)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }
}

