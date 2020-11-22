using Assets.Scripts.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects {
    public class MergableObjectManager : MonoBehaviour
    {

        public static MergableObjectManager Instance {
            get {
                if (_Instance == null) { _Instance = FindObjectOfType<MergableObjectManager> (); }
                return _Instance;
            }
        }
        private static MergableObjectManager _Instance { get; set; }


        public float baseMergeRadius = 0.25f;
        public int countForMerge = 2;
        public float minimumScale = 30f;    
        public bool mergeOnlyEqualSizes = false;

        [SerializeField]
        private bool debugMergeRadii;

        public void OnEnable()
        {
            StartCoroutine(IntermittentCheckForMerges());
        }

        System.Collections.IEnumerator IntermittentCheckForMerges() {
            WaitForSeconds wait = new WaitForSeconds(0.1f);
            while(true)
            {
                if (mergeOnlyEqualSizes)
                    CheckForMergesBySize();
                else
                    CheckForMerges(MergableObject.All);

                yield return wait;
            }
        }


        void CheckForMergesBySize () {
            var allBySize = MergableObject.AllBySize;
            int[] sizes = MergableObject.Sizes;

            foreach (int size in sizes) {
                MergableObject[] all = allBySize[size];
                CheckForMerges (all);
            }
        }


        void CheckForMerges (MergableObject[] all) {
           // Debug.Log("Checking for merges");
            all = all.Where (t => t.CanMerge).ToArray ();
            if (all.Length < countForMerge) { return; }

            List<MergableObject> unused = all.ToList ();
            foreach (MergableObject t in all) {
                if (!unused.Contains (t)) { continue; }
                float mergeRadius = baseMergeRadius * t.transform.localScale.x / minimumScale;
                var inRange = unused.Where (u => Vector3.Distance (u.transform.position, t.transform.position) < mergeRadius).ToArray ();
                if (inRange.Length < countForMerge) { continue; }

                MergableObject[] merging = inRange.Take (countForMerge).ToArray ();
                Merge (merging);
                foreach (var merged in merging) { unused.Remove (merged); }

                //Debug.Log ($"{unused.Count} tetrahedra remaining in merging group");
            }
        }

        public void OnDrawGizmos()
        {
            if(debugMergeRadii)
            {
                MergableObject[] all = MergableObject.All.Where(t => t.CanMerge).ToArray();

                Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
                foreach (MergableObject obj in all)
                {
                    float mergeRadius = baseMergeRadius * obj.transform.localScale.x / minimumScale;
                    Gizmos.DrawSphere(obj.transform.transform.position, mergeRadius);
                }
            }
        }


        void Merge (MergableObject[] toMerge) {
            string debugSizes = toMerge.Select(t => t.GetComponent<ObjectSize> ().Size.ToString ()).Aggregate (string.Empty, (s, acc) => s + acc + ", ");
            Debug.Log ($"Merging sizes: {debugSizes}");

            int mergedSize = MergedSize (toMerge);
            toMerge[0].SetSize (mergedSize);

            //Vector3 mergedPosition = MergedTetrahedronPosition (toMerge, mergedSize);
            //Debug.Log("Merged pos is " + mergedPosition + " source pos is " + toMerge[0].transform.position);
           // toMerge[0].transform.position = mergedPosition;

            GameObject mergeParticles = ObjectPool.Instance.GetObjectForType("TriangularImplosion");
            mergeParticles.transform.position = toMerge[0].transform.position;

            for (int i = 1; i < toMerge.Length; i++) {
                PoolShape (toMerge[i]);
            }
        }


      /*  Vector3 MergedTetrahedronPosition (MergableObject[] tetrahedra, int mergedSize) {
            return tetrahedra
                .Select (t => t.transform.position * t.GetComponent<ObjectSize> ().Size)
                .Aggregate (Vector3.zero, (p, acc) => acc + p) / mergedSize;
        }*/


        int MergedSize (MergableObject[] tetrahedra) {
            return tetrahedra
                .Select (t => t.GetComponent<ObjectSize> ().Size)
                .Aggregate (0, (size, acc) => size + acc);
        }

        void PoolShape (MergableObject tetrahedron) {
            Utility.ObjectPool.Instance.PoolObject (tetrahedron.gameObject);
            //Destroy (tetrahedron.gameObject);
        }
    }
}
