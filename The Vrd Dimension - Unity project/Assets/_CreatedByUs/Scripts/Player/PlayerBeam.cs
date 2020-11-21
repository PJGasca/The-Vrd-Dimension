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
        private GameObject laserPointer;

        [SerializeField]
        private GameObject attractBeam;

        [SerializeField]
        private GameObject repelBeam;

        [SerializeField]
        private GameObject holdBeam;

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
                        laserPointer.SetActive(false);
                        repelBeam.SetActive(false);
                        attractBeam.SetActive(true);
                        holdBeam.SetActive(false);
                        break;

                    case BeamMode.REPEL:
                        laserPointer.SetActive(false);
                        repelBeam.SetActive(true);
                        attractBeam.SetActive(false);
                        holdBeam.SetActive(false);
                        break;

                    case BeamMode.BOTH:
                        laserPointer.SetActive(false);
                        repelBeam.SetActive(false);
                        attractBeam.SetActive(false);
                        holdBeam.SetActive(true);
                        break;

                    case BeamMode.NEUTRAL:
                        laserPointer.SetActive(true);
                        repelBeam.SetActive(false);
                        attractBeam.SetActive(false);
                        holdBeam.SetActive(false);
                        break;

                    default:
                        laserPointer.SetActive(false);
                        repelBeam.SetActive(false);
                        attractBeam.SetActive(false);
                        holdBeam.SetActive(false);
                        break;
                }
            }
        }

        public void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            ObjectsInBeam.Add(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            ObjectsInBeam.Remove(other.gameObject);
        }
    }
}

