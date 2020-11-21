using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(LineRenderer))]
    public class CrackleBeam : MonoBehaviour
    {
        [SerializeField]
        private float lineLength;

        [SerializeField]
        private float maxCrackle;

        private LineRenderer line;

        public void OnEnable()
        {
            line = GetComponent<LineRenderer>();

            int totalPoints = line.positionCount;
            float zPos = 0;
            float zIncrement = lineLength / totalPoints;

            Vector3[] points = new Vector3[totalPoints];
            line.GetPositions(points);

            for(int i = 0; i < totalPoints; i++)
            {
                points[i].z = zPos;
                zPos += zIncrement;
            }

            line.SetPositions(points);

            for(int i = 1; i < totalPoints; i++)
            {
                StartCoroutine(MovePoint(i));
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private IEnumerator MovePoint(int pointIndex)
        {
            while(true)
            {
                Vector3 point = line.GetPosition(pointIndex);
                Vector3 startPos = new Vector3(0, 0, point.z);
                line.SetPosition(pointIndex, startPos);
                Vector3 destPos = new Vector3(Random.Range(-maxCrackle, maxCrackle), Random.Range(-maxCrackle, maxCrackle), point.z);
                float time = Random.Range(0.2f, 0.5f);
                float elapsed = 0f;
                while(line.GetPosition(pointIndex)!=destPos)
                {
                    yield return null;
                    elapsed += Time.deltaTime;
                    Vector3 newPos = Vector3.Lerp(startPos, destPos, elapsed / time);
                    line.SetPosition(pointIndex, newPos);
                }

            }
        }
    }
}

