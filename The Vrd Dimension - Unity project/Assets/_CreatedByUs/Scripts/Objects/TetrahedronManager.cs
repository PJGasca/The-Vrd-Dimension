using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects {
    public class TetrahedronManager : MonoBehaviour
    {
        private const float mergeRadius = 0.5f;

        public static TetrahedronManager Instance {
            get {
                if (_Instance == null) { _Instance = FindObjectOfType<TetrahedronManager> (); }
                return _Instance;
            }
        }
        private static TetrahedronManager _Instance { get; set; }


        public int countForMerge = 2;
        public float minimumScale = 30f;    
        public bool mergeOnlyEqualSizes = false;

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
                    CheckForMerges(Tetrahedron.All);

                yield return wait;
            }
        }


        void CheckForMergesBySize () {
            var allBySize = Tetrahedron.AllBySize;
            int[] sizes = Tetrahedron.Sizes;

            foreach (int size in sizes) {
                Tetrahedron[] all = allBySize[size];
                CheckForMerges (all);
            }
        }


        void CheckForMerges (Tetrahedron[] all) {
            Debug.Log("Checking for merges");
            all = all.Where (t => t.CanMerge).ToArray ();
            if (all.Length < countForMerge) { return; }

            List<Tetrahedron> unused = all.ToList ();
            foreach (Tetrahedron t in all) {
                if (!unused.Contains (t)) { continue; }

                var inRange = unused.Where (u => Vector3.Distance (u.transform.position, t.transform.position) < mergeRadius).ToArray ();
                if (inRange.Length < countForMerge) { continue; }

                Tetrahedron[] merging = inRange.Take (countForMerge).ToArray ();
                Merge (merging);
                foreach (var merged in merging) { unused.Remove (merged); }

                Debug.Log ($"{unused.Count} tetrahedra remaining in merging group");
            }
        }


        void Merge (Tetrahedron[] tetrahedra) {
            string debugSizes = tetrahedra.Select(t => t.GetComponent<ObjectSize> ().Size.ToString ()).Aggregate (string.Empty, (s, acc) => s + acc + ", ");
            Debug.Log ($"Merging sizes: {debugSizes}");

            int mergedSize = MergedTetrahedronSize (tetrahedra);
            tetrahedra[0].SetSize (mergedSize);

            Vector3 mergedPosition = MergedTetrahedronPosition (tetrahedra, mergedSize);
            tetrahedra[0].transform.position = mergedPosition;
            // TODO: Instantiate particle prefab at mergedPosition

            for (int i = 1; i < tetrahedra.Length; i++) {
                PoolTetrahedron (tetrahedra[i]);
            }
        }


        Vector3 MergedTetrahedronPosition (Tetrahedron[] tetrahedra, int mergedSize) {
            return tetrahedra
                .Select (t => t.transform.position * t.GetComponent<ObjectSize> ().Size)
                .Aggregate (Vector3.zero, (p, acc) => acc + p) / mergedSize;
        }


        int MergedTetrahedronSize (Tetrahedron[] tetrahedra) {
            return tetrahedra
                .Select (t => t.GetComponent<ObjectSize> ().Size)
                .Aggregate (0, (size, acc) => size + acc);
        }

        void PoolTetrahedron (Tetrahedron tetrahedron) {
            Utility.ObjectPool.Instance.PoolObject (tetrahedron.gameObject);
            //Destroy (tetrahedron.gameObject);
        }
    }
}
