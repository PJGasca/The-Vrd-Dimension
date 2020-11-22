using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects {
    public class TetrahedronManager : MonoBehaviour
    {
        public const int countForMerge = 4;
        public const float mergeRadius = 0.5f;


        void FixedUpdate () {
            CheckForMerges ();
        }


        void CheckForMerges () {
            var allBySize = Tetrahedron.AllBySize;
            int[] sizes = Tetrahedron.Sizes;
            Debug.Log ("Size count for merge: " + sizes.Length);
            foreach (int size in sizes) {
                Tetrahedron[] all = allBySize[size];
                Debug.Log ($"{all.Length} tetrahedra of size {size}");
                CheckForMerges (all);
            }

            Debug.Log ($"Remaining tetrahedra: {Tetrahedron.All.Length}");
        }
        void CheckForMerges (Tetrahedron[] all) {
            if (all.Length < countForMerge) { return; }

            List<Tetrahedron> unused = all.ToList ();
            foreach (Tetrahedron t in all) {
                if (!unused.Contains (t)) { continue; }

                var inRange = unused.Where (u => Vector3.Distance (u.transform.position, t.transform.position) < mergeRadius).ToArray ();
                if (inRange.Length < countForMerge) { continue; }

                Debug.Log ("Merging!");

                Tetrahedron[] merging = inRange.Take (countForMerge).ToArray ();
                Merge (merging);
                // unused.RemoveAll (u => merging.Contains (u));
                foreach (var merged in merging) { unused.Remove (merged); }
            }
        }


        void Merge (Tetrahedron[] tetrahedra) {            
            int newSize = tetrahedra[0].GetComponent<ObjectSize> ().Size * countForMerge;
            tetrahedra[0].SetSize (newSize);
            Vector3 mergedPosition = MergedTetrahedronPosition (tetrahedra);
            tetrahedra[0].transform.position = mergedPosition;
            // TODO: Instantiate particle prefab at mergedPosition

            for (int i = 1; i < tetrahedra.Length; i++) {
                PoolTetrahedron (tetrahedra[i]);
            }
        }


        Vector3 MergedTetrahedronPosition (Tetrahedron[] tetrahedra) {
            return tetrahedra
                .Select (t => t.transform.position)
                .Aggregate (Vector3.zero, (p, acc) => acc + p) / countForMerge;
        }


        // TODO: ENABLE POOLING
        void PoolTetrahedron (Tetrahedron tetrahedron) {
            // Utility.ObjectPool.Instance.PoolObject (tetrahedron.gameObject);
            Destroy (tetrahedron.gameObject);
        }
    }
}
