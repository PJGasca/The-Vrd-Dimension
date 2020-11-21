using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects {
    public class TetrahedronManager : MonoBehaviour
    {
        public const int countForMerge = 4;
        public const float mergeRadius = 0.5f;


        void FixedUpdate (){
            CheckForMerges ();
        }


        void CheckForMerges () {
            var allBySize = Tetrahedron.AllBySize;
            int[] sizes = Tetrahedron.Sizes;
            foreach (int size in sizes) {
                Tetrahedron[] all = allBySize[size];
                CheckForMerges (all);
            }
        }
        void CheckForMerges (Tetrahedron[] all) {
            if (all.Length < countForMerge) { return; }

            List<Tetrahedron> unused = all.ToList ();
            foreach (Tetrahedron t in all) {
                if (!unused.Contains (t)) { continue; }

                var inRange = unused.Where (u => Vector3.Distance (u.transform.position, t.transform.position) < mergeRadius).ToArray ();
                if (inRange.Length < countForMerge) { continue; }

                Tetrahedron[] merging = inRange.Take (countForMerge).ToArray ();
                Merge (merging);
                unused.RemoveAll (u => merging.Contains (u));
            }
        }


        void Merge (Tetrahedron[] tetrahedra) {            
            int newSize = tetrahedra[0].GetComponent<ObjectSize> ().Size * countForMerge;
            tetrahedra[0].SetSize (newSize);
            tetrahedra[0].transform.position = tetrahedra
                .Select (t => t.transform.position)
                .Aggregate (Vector3.zero, (p, acc) => acc + p) / countForMerge;

            for (int i = 1; i < tetrahedra.Length; i++) {
                Utility.ObjectPool.Instance.PoolObject (tetrahedra[i].gameObject);
            }
        }
    }
}
