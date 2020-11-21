using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerBeam : MonoBehaviour
    {
        public HashSet<GameObject> ObjectsInBeam { get; private set; } = new HashSet<GameObject>();

        public enum BeamMode { ATTRACT, REPEL, BOTH, NEUTRAL, OFF };

        [SerializeField]
        private Color neutralColor;

        [SerializeField]
        private Color attractingColor;

        [SerializeField]
        private Color repellingColor;

        [SerializeField]
        private Color bothColor;

        private Color invisible = new Color(0, 0, 0, 0);

        private BeamMode mode = BeamMode.NEUTRAL;

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

                switch (mode)
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

                    case BeamMode.NEUTRAL:
                        SetBeamColor(neutralColor);
                        break;

                    default:
                        SetBeamColor(invisible);
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
            SetBeamColor(neutralColor);
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

