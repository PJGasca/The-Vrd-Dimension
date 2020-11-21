using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects {
    public class TetrahedronManager : MonoBehaviour
    {
        public const int countForMerge = 4;
        public const float mergeDistance = 0.5f;


        private Tetrahedron[] _xOrdered;
        private Tetrahedron[] _yOrdered;
        private Tetrahedron[] _zOrdered;
        private float[] _xDifferences;
        private float[] _yDifferences;
        private float[] _zDifferences;


        void FixedUpdate (){
            CheckForMerges ();
        }


        void CheckForMerges () {
            int[] sizes = Tetrahedron.Sizes;
            foreach (int size in sizes) {
                Tetrahedron[] all = Tetrahedron.AllBySize (size);
                CheckForMerges (all);
            }
        }
        void CheckForMerges (Tetrahedron[] all) {
            if (all.Length < countForMerge) { return; }

            _xOrdered = all.OrderBy ((t) => t.transform.position.x).ToArray ();
            // _yOrdered = all.OrderBy ((t) => t.transform.position.y).ToArray ();
            // _zOrdered = all.OrderBy ((t) => t.transform.position.z).ToArray ();

            _xDifferences = new float[_xOrdered.Length - countForMerge + 1];
            for (int i = 0; i < _xDifferences.Length; i++) {
                _xDifferences[i] = _xOrdered[i + countForMerge - 1].transform.position.x - _xOrdered[i].transform.position.y;
                
            }
        }
    }
}
