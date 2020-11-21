using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects {
    [RequireComponent (typeof (Grabbable), typeof (ObjectSize))]
    public class Tetrahedron : MonoBehaviour
    {
#region Constants and static members
        public const float minimumScale = 2f;
        public const float baseSplitForce = 10f;


        public static Dictionary<int, Tetrahedron[]> AllBySize {
            get {
                Dictionary<int, Tetrahedron[]> allBySize = new Dictionary<int, Tetrahedron[]> ();
                foreach (int key in _allBySize.Keys) {
                    if (_allBySize[key].Count > 0) { allBySize.Add (key, _allBySize[key].ToArray ()); }
                }
                return allBySize;
            }
        }
        public static int[] Sizes => _sizes.ToArray ();


        private static Dictionary<int, List<Tetrahedron>> _allBySize = new Dictionary<int, List<Tetrahedron>> ();
        private static List<int> _sizes = new List<int> ();


        public static Tetrahedron[] AllForSize (int size) {
            if (!_allBySize.ContainsKey (size) || _allBySize[size].Count == 0) { return new Tetrahedron[0]; }
            return _allBySize[size].ToArray ();
        }
#endregion


#region Instance members

        public event System.Action<Tetrahedron> OnTetrahedronEnabled;
        public event System.Action<Tetrahedron> OnTetrahedronDisabled;
        public event System.Action<Tetrahedron> OnTetrahedronSplit;


        public Vector3 SpawnPosition { get; private set; }


        private ObjectSize _sizeComponent;


        void Awake () {
            SetSize (GetComponent<ObjectSize> ().Size);
            SpawnPosition = transform.position;
        }


        void OnEnable () {
            Game.GameManager.Instance?.OnObjectAdded (this.gameObject);
            if (OnTetrahedronEnabled != null) { OnTetrahedronEnabled (this); }
        }


        void OnDisable () {
            _allBySize[_sizeComponent.Size].Remove (this);
            Game.GameManager.Instance?.OnObjectRemoved (this.gameObject);
            if (OnTetrahedronDisabled != null) { OnTetrahedronDisabled (this); }
        }


        public void Split () {
            if (_sizeComponent.Size < TetrahedronManager.countForMerge) { return; }

            Tetrahedron[] newTetrahedra = new Tetrahedron[TetrahedronManager.countForMerge];
            newTetrahedra[0] = this;
            for (int i = 1; i < TetrahedronManager.countForMerge; i++) {
                newTetrahedra[i] = Utility.ObjectPool.Instance.GetObjectForType ("Tetrahedron", true).GetComponent<Tetrahedron> ();
            }

            Vector3 center = transform.position;
            int newSize = _sizeComponent.Size / TetrahedronManager.countForMerge;

            Vector3[] vertices = GetComponent<MeshFilter> ().mesh.vertices
                .Select (v => transform.TransformPoint (v))
                .ToArray ();
            Vector3[] newPositions = vertices
                .Select (v => Vector3.Normalize (v - center) * 1.2f * TetrahedronManager.mergeRadius)
                .ToArray ();
            Vector3[] velocities = vertices
                .Select (v => baseSplitForce * Vector3.Normalize (v - center) / _sizeComponent.Size)
                .ToArray ();

            for (int i = 0; i < newTetrahedra.Length; i++) {
                newTetrahedra[i].transform.position = newPositions[i];
                newTetrahedra[i].GetComponent<Rigidbody> ().velocity = velocities[i];
                newTetrahedra[i].SetSize (newSize);
                newTetrahedra[i].gameObject.SetActive (true);
            }

            if (OnTetrahedronSplit != null) { OnTetrahedronSplit (this); }
        }


        public void SetSize (int newSize) {
            int oldSize = _sizeComponent.Size;
            _sizeComponent.Size = newSize;

            SetScale (newSize);
            RegisterBySize (newSize, oldSize);
        }


        void SetScale (int newSize) {
            transform.localScale = minimumScale * Mathf.Log (newSize, 4) * Vector3.one;
        }


        void RegisterBySize (int newSize, int oldSize) {
            if (!_allBySize.ContainsKey (newSize)) { _allBySize.Add (newSize, new List<Tetrahedron> ()); }
            _allBySize[newSize].Add (this);
            if (!_sizes.Contains (newSize)) { _sizes.Add (newSize); }

            if (!_allBySize.ContainsKey (oldSize)) { return; }
            if (_allBySize[oldSize].Contains (this)) { _allBySize[oldSize].Remove (this); }
            if (_allBySize[oldSize].Count == 0) { _sizes.Remove (oldSize); }
        }

        public bool IsDisplaced()
        {
            // TODO: Work out if we have moved from our start position
            return true;
        }
#endregion
    }
}
