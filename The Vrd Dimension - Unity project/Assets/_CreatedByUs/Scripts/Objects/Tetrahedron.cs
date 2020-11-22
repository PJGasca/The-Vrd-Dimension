using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects {
    [RequireComponent (typeof (Grabbable), typeof (ObjectSize))]
    public class Tetrahedron : MonoBehaviour
    {
#region Constants and static members
        public const float minimumScale = 30f;
        public const float baseSplitVelocity = 0.5f;


        public static Tetrahedron[] All => _all.ToArray ();
        public static Dictionary<int, Tetrahedron[]> AllBySize {
            get {
                Dictionary<int, Tetrahedron[]> allBySize = new Dictionary<int, Tetrahedron[]> ();
                foreach (int key in _allBySize.Keys) {
                    if (_allBySize[key].Count > 0) { allBySize.Add (key, _allBySize[key].ToArray ()); }
                }
                return allBySize;
            }
        }
        public static int[] Sizes => AllBySize.Where (kvp => kvp.Value.Length > 0).Select (kvp => kvp.Key).ToArray ();


        private static List<Tetrahedron> _all = new List<Tetrahedron> ();
        private static Dictionary<int, List<Tetrahedron>> _allBySize = new Dictionary<int, List<Tetrahedron>> ();


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

        public bool targetedByAgent;


        [SerializeField] private ObjectSize _sizeComponent;

        void Awake () {
            if (_sizeComponent == null) { _sizeComponent = GetComponent<ObjectSize> (); }
        }


        void OnEnable () {
            SpawnPosition = transform.position;
            _all.Add (this);
            InitializeSize ();
            Game.GameManager.Instance?.OnObjectAdded (this.gameObject);
            if (OnTetrahedronEnabled != null) { OnTetrahedronEnabled (this); }
        }


        void OnDisable () {
            targetedByAgent = false;
            _all.Remove (this);
            _allBySize[_sizeComponent.Size].Remove (this);
            Game.GameManager.Instance?.OnObjectRemoved (this.gameObject);
            if (OnTetrahedronDisabled != null) { OnTetrahedronDisabled (this); }
        }


        public void Split () {
            if (_sizeComponent.Size < TetrahedronManager.countForMerge) { return; }

            Tetrahedron[] newTetrahedra = TetrahedraForSplit ();

            Vector3 center = transform.position;
            int newSize = _sizeComponent.Size / TetrahedronManager.countForMerge;

            Vector3[] vertices = VertexOffsets (center);

            for (int i = 0; i < newTetrahedra.Length; i++) {
                newTetrahedra[i].transform.position = PositionForSplit (vertices[i], center);
                newTetrahedra[i].GetComponent<Rigidbody> ().velocity = VelocityForSplit (vertices[i]);
                newTetrahedra[i].SetSize (newSize, _sizeComponent.Size);
                newTetrahedra[i].gameObject.SetActive (true);
            }

            if (OnTetrahedronSplit != null) { OnTetrahedronSplit (this); }
        }


        public void SetSize (int newSize) {
            SetSize (newSize, _sizeComponent.Size);
        }
        public void SetSize (int newSize, int oldSize) {
            _sizeComponent.Size = newSize;

            SetScale (newSize);
            RegisterBySize (newSize, oldSize);
        }


        void InitializeSize () {
            SetSize (_sizeComponent.Size, -1);
        }


        void SetScale (int newSize) {
            transform.localScale = minimumScale * (1 + Mathf.Log (newSize, 4)) * Vector3.one;
        }


        Tetrahedron[] TetrahedraForSplit () {
            Tetrahedron[] newTetrahedra = new Tetrahedron[TetrahedronManager.countForMerge];
            newTetrahedra[0] = this;
            for (int i = 1; i < TetrahedronManager.countForMerge; i++) {
                newTetrahedra[i] = GetTetrahedronFromPool ();
            }

            return newTetrahedra;
        }


        Vector3[] VertexOffsets (Vector3 center) {
            return GetComponent<MeshFilter> ().mesh.vertices
                .Select (v => transform.TransformPoint (v))
                .Select (v => (v - center).normalized)
                .ToArray ();
        }


        Vector3 PositionForSplit (Vector3 normalizedOffset, Vector3 center) {
            return center + normalizedOffset * 1.2f * TetrahedronManager.mergeRadius;
        }


        Vector3 VelocityForSplit (Vector3 normalizedOffset) {
            return normalizedOffset * baseSplitVelocity / _sizeComponent.Size;
        }


        void RegisterBySize (int newSize, int oldSize) {
            if (!_allBySize.ContainsKey (newSize)) { _allBySize.Add (newSize, new List<Tetrahedron> ()); }
            _allBySize[newSize].Add (this);

            if (!_allBySize.ContainsKey (oldSize)) { return; }
            if (_allBySize[oldSize].Contains (this)) { _allBySize[oldSize].Remove (this); }
        }


        // TODO: ENABLE POOLING
        Tetrahedron GetTetrahedronFromPool () {
            return Utility.ObjectPool.Instance.GetObjectForType ("Tetrahedron", true).GetComponent<Tetrahedron> ();
            //return Instantiate (this);
        }

#endregion
    }
}
