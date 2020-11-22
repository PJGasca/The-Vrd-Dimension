using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects {
    [RequireComponent (typeof (Grabbable), typeof (ObjectSize))]
    public class Tetrahedron : MonoBehaviour
    {
#region Constants and static members

        public const float baseSplitVelocity = 0.5f;
        public const float splitRadius = 0.01f;


        private const float _mergeCooldown = 2f;


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
        public event System.Action<Tetrahedron> OnTetrahedronSplit;


        public Vector3 SpawnPosition { get; private set; }
        public bool CanMerge => _HasMovedFromScenePosition && !_OnMergeCooldown;


        private bool _OnMergeCooldown { get; set; }
        private bool _HasMovedFromScenePosition { get; set; }


        public bool targetedByAgent;


        [SerializeField] private ObjectSize _sizeComponent;
        private Coroutine _mergeTimerCoroutine;
        private bool _didWatchPositionCheck;


        void Awake () {
            if (_sizeComponent == null) { _sizeComponent = GetComponent<ObjectSize> (); }
        }


        void OnEnable () {
            SpawnPosition = transform.position;
            _all.Add (this);
            Game.GameManager.Instance?.OnObjectAdded (this.gameObject);
            InitializeSize ();

            if (OnTetrahedronEnabled != null) { OnTetrahedronEnabled (this); }
        }


        void OnDisable () {
            targetedByAgent = false;
            if (_all.Contains (this)) { _all.Remove (this); }
            if (_allBySize.ContainsKey (_sizeComponent.Size) && _allBySize[_sizeComponent.Size].Contains (this)) {
                _allBySize[_sizeComponent.Size].Remove (this);
            }
            Game.GameManager.Instance?.OnObjectRemoved (this.gameObject);

            if (_didWatchPositionCheck) { _HasMovedFromScenePosition = true; }
            else { Debug.LogWarning ("Tetrahedron called OnDisable before calling Start()!"); }
        }


        void Start () {
            StartCoroutine (WatchPositionInScene ());
        }


        public void Split () {
            if (_sizeComponent.Size < TetrahedronManager.Instance.countForMerge) { return; }

            GameObject particles = ObjectPool.Instance.GetObjectForType("SplitParticles");
            particles.transform.position = transform.position;

            Tetrahedron[] newTetrahedra = TetrahedraForSplit ();

            Vector3 center = transform.position;
            int[] sizes = SplitSizes ();

            Vector3[] vertices = VertexOffsets (center);

            for (int i = 0; i < newTetrahedra.Length; i++) {
                newTetrahedra[i].transform.position = PositionForSplit (vertices[i], center);
                // newTetrahedra[i].GetComponent<Rigidbody> ().velocity = VelocityForSplit (vertices[i]);
                newTetrahedra[i].SetSize (sizes[i], _sizeComponent.Size);
                newTetrahedra[i].gameObject.SetActive (true);
            }

            if (OnTetrahedronSplit != null) { OnTetrahedronSplit (this); }
        }


        public void SetSize (int newSize) {
            SetSize (newSize, _sizeComponent.Size);
        }
        public void SetSize (int newSize, int oldSize) {
            StartMergeCooldownTimer ();
            _sizeComponent.Size = newSize;

            SetScale (newSize);
            RegisterBySize (newSize, oldSize);
        }


        void InitializeSize () {
            SetSize (_sizeComponent.Size, -1);
        }


        void StartMergeCooldownTimer () {
            if (_mergeTimerCoroutine != null) { return; }

            _OnMergeCooldown = true;
            _mergeTimerCoroutine = StartCoroutine (RunMergeCooldownTimer ());            
        }


        IEnumerator RunMergeCooldownTimer () {
            yield return new WaitForSeconds (_mergeCooldown);
            _OnMergeCooldown = false;
            _mergeTimerCoroutine = null;
        }


        void SetScale (int newSize) {
            TetrahedronManager manager = TetrahedronManager.Instance;
            transform.localScale = manager.minimumScale * (1 + Mathf.Log (newSize, manager.countForMerge)) * Vector3.one;
        }


        Tetrahedron[] TetrahedraForSplit () {
            Tetrahedron[] newTetrahedra = new Tetrahedron[TetrahedronManager.Instance.countForMerge];
            newTetrahedra[0] = this;
            for (int i = 1; i < TetrahedronManager.Instance.countForMerge; i++) {
                newTetrahedra[i] = GetTetrahedronFromPool ();
            }

            return newTetrahedra;
        }


        int[] SplitSizes () {
            int[] sizes = new int[TetrahedronManager.Instance.countForMerge];

            int average = _sizeComponent.Size / TetrahedronManager.Instance.countForMerge;
            int remainder = _sizeComponent.Size % TetrahedronManager.Instance.countForMerge;

            sizes[0] = average + remainder;
            for (int i = 1; i < sizes.Length; i++) { sizes[i] = average; }

            return sizes;
        }


        Vector3[] VertexOffsets (Vector3 center) {
            return GetComponent<MeshFilter> ().mesh.vertices
                .Select (v => transform.TransformPoint (v))
                .Select (v => (v - center).normalized)
                .ToArray ();
        }


        Vector3 PositionForSplit (Vector3 normalizedOffset, Vector3 center) {
            return center + normalizedOffset * splitRadius;
        }


        // Vector3 VelocityForSplit (Vector3 normalizedOffset) {
        //     return normalizedOffset * baseSplitVelocity / _sizeComponent.Size;
        // }


        void RegisterBySize (int newSize, int oldSize) {
            if (!_allBySize.ContainsKey (newSize)) { _allBySize.Add (newSize, new List<Tetrahedron> ()); }
            _allBySize[newSize].Add (this);

            if (!_allBySize.ContainsKey (oldSize)) { return; }
            if (_allBySize[oldSize].Contains (this)) { _allBySize[oldSize].Remove (this); }
        }


        IEnumerator WatchPositionInScene () {
            _didWatchPositionCheck = true;
            Vector3 positionInScene = transform.position;
            do {
                yield return new WaitForEndOfFrame ();
            } while (Vector3.Distance (transform.position, positionInScene) < 0.05f);
            
            _HasMovedFromScenePosition = true;
        }


        // TODO: ENABLE POOLING
        Tetrahedron GetTetrahedronFromPool () {
            return Utility.ObjectPool.Instance.GetObjectForType ("Tetrahedron", true).GetComponent<Tetrahedron> ();
            //return Instantiate (this);
        }

#endregion
    }
}
