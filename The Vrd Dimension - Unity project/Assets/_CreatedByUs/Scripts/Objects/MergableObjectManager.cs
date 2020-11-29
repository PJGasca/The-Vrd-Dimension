using Assets.Scripts.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects {
    public class MergableObjectManager : MonoBehaviour
    {
        public static event System.Action<int> OnMergeUpdate;


        public static MergableObjectManager Instance {
            get {
                if (_Instance == null) { _Instance = FindObjectOfType<MergableObjectManager> (); }
                return _Instance;
            }
        }
        private static MergableObjectManager _Instance { get; set; }


        //public float baseMergeRadius = 0.25f;
        public int countForMerge = 2;
        public float minimumScale = 30f;    
        public bool mergeOnlyEqualSizes = false;

        [SerializeField]
        private bool debugMergeRadii;
        private Coroutine mergeCoroutine;

        public void OnEnable()
        {
            Game.EndChecker.OnGameWin += DoBigBang;
            mergeCoroutine = StartCoroutine(IntermittentCheckForMerges());
        }

        void OnDisable () {
            Game.EndChecker.OnGameWin -= DoBigBang;
            StopMergeCoroutine ();
        }

        System.Collections.IEnumerator IntermittentCheckForMerges() {
            WaitForSeconds wait = new WaitForSeconds(0.1f);
            while(true)
            {
                yield return wait;
                if (mergeOnlyEqualSizes)
                    CheckForMergesBySize();
                else
                    CheckForMerges(MergableObject.All);

                if (OnMergeUpdate != null) { OnMergeUpdate (MergableObject.Count); }
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


        void CheckForMerges (MergableObject[] all)
        {
           // Debug.Log("Checking for merges");
            all = all.Where (t => t.CanMerge).ToArray ();
            if (all.Length < countForMerge) { return; }

            List<MergableObject> unused = all.ToList ();
            foreach (MergableObject t in all) 
            {
                if (!unused.Contains (t)) { continue; }
               
                HashSet<MergableObject> inRange = t.OverlappingMergeTriggers;
                if (inRange.Count + 1 < countForMerge) { continue; }
                List<MergableObject> toMerge = new List<MergableObject>(inRange);
                toMerge.Add(t);
                MergableObject[] merging = toMerge.ToArray();

                Merge (merging);
                foreach (var merged in merging) { unused.Remove (merged); }

                //Debug.Log ($"{unused.Count} tetrahedra remaining in merging group");
            }
        }

        /*public void OnDrawGizmos()
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
        }*/


        void Merge (MergableObject[] toMerge) {
            string debugSizes = toMerge.Select(t => t.Size.ToString ()).Aggregate (string.Empty, (s, acc) => s + acc + ", ");
            Debug.Log ($"Merging sizes: {debugSizes}");

            int mergedSize = MergedSize (toMerge);
            toMerge[0].SetSize (mergedSize);

            //Vector3 mergedPosition = MergedTetrahedronPosition (toMerge, mergedSize);
            //Debug.Log("Merged pos is " + mergedPosition + " source pos is " + toMerge[0].transform.position);
           // toMerge[0].transform.position = mergedPosition;

            GameObject mergeParticles = ObjectPool.Instance.GetObjectForType("TriangularImplosion");
            mergeParticles.transform.position = toMerge[0].transform.position;
            toMerge[0].GetComponent<AudioSource>().PlayOneShot(SoundEffectClips.instance.objectMerge[Random.Range(0, SoundEffectClips.instance.objectMerge.Count)], 2);

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
                .Select (t => t.Size)
                .Aggregate (0, (size, acc) => size + acc);
        }

        void PoolShape (MergableObject tetrahedron) {
            Utility.ObjectPool.Instance.PoolObject (tetrahedron.gameObject);
            //Destroy (tetrahedron.gameObject);
        }

        void DoBigBang () {
            foreach (MergableObject mergeableObject in MergableObject.All) {
                mergeableObject.Explode ();
            }

            StopMergeCoroutine ();
        }


        void StopMergeCoroutine () {
            if (mergeCoroutine != null) {
                StopCoroutine (mergeCoroutine);
                mergeCoroutine = null;
            }
        }
    }
}
