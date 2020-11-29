using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects {
    [RequireComponent (typeof (Grabbable))]
    public class MergableObject : MonoBehaviour
    {
        public enum ShapeType { Tetrahedron, Octohedron }

        public HashSet<MergableObject> OverlappingMergeTriggers
        {
            get; private set;
        } = new HashSet<MergableObject>();

#region Constants and static members

        public const float baseSplitVelocity = 0.5f;
        public const float splitRadius = 0.01f;


        private const float _mergeCooldown = 2f;


        public static int Count => _all.Count;
        public static MergableObject[] All => _all.ToArray ();
        public static Dictionary<int, MergableObject[]> AllBySize {
            get {
                Dictionary<int, MergableObject[]> allBySize = new Dictionary<int, MergableObject[]> ();
                foreach (int key in _allBySize.Keys) {
                    if (_allBySize[key].Count > 0) { allBySize.Add (key, _allBySize[key].ToArray ()); }
                }
                return allBySize;
            }
        }
        public static int[] Sizes => AllBySize.Where (kvp => kvp.Value.Length > 0).Select (kvp => kvp.Key).ToArray ();


        private static List<MergableObject> _all = new List<MergableObject> ();
        private static Dictionary<int, List<MergableObject>> _allBySize = new Dictionary<int, List<MergableObject>> ();


        public static MergableObject[] AllForSize (int size) {
            if (!_allBySize.ContainsKey (size) || _allBySize[size].Count == 0) { return new MergableObject[0]; }
            return _allBySize[size].ToArray ();
        }
#endregion


#region Instance members

        public event System.Action<MergableObject> OnShapeEnabled;
        public event System.Action<MergableObject> OnShapeSplit;
        public event System.Action<MergableObject, int> OnSizeChanged;



        public Vector3 SpawnPosition { get; private set; }
        public bool CanMerge => _HasMovedFromScenePosition && !_OnMergeCooldown;


        private bool _OnMergeCooldown { get; set; }
        private bool _HasMovedFromScenePosition { get; set; }

        [SerializeField]
        private int _size;

        public int Size
        {
            get
            {
                return _size;
            }

            private set
            {
                _size = value;

                SetScale(_size);
                rb.mass = _size;

                if (OnSizeChanged != null)
                {
                    OnSizeChanged(this, _size);
                }
            }
        }


        public bool targetedByAgent;

        [SerializeField]
        private ShapeType shapeType = ShapeType.Tetrahedron;

        private Coroutine _mergeTimerCoroutine;
        private bool _didWatchPositionCheck;

        private Rigidbody rb;


        void Awake () {
            rb = GetComponent<Rigidbody>();
        }

        void OnEnable () {
            targetedByAgent = false;
            SpawnPosition = transform.position;
            _all.Add (this);
            Game.GameManager.Instance?.OnObjectAdded (this.gameObject);
            InitializeSize ();

            if (OnShapeEnabled != null) { OnShapeEnabled (this); }
        }


        void OnDisable () {
            OverlappingMergeTriggers.Clear();
            targetedByAgent = false;
            if (_all.Contains (this)) { _all.Remove (this); }
            if (_allBySize.ContainsKey (Size) && _allBySize[Size].Contains (this)) {
                _allBySize[Size].Remove (this);
            }
            Game.GameManager.Instance?.OnObjectRemoved (this.gameObject);

            if (_didWatchPositionCheck) { _HasMovedFromScenePosition = true; }
            else { Debug.LogWarning ("Shape called OnDisable before calling Start()!"); }
        }


        void Start () {
            StartCoroutine (WatchPositionInScene ());
        }


        public void Split () {
            if (Size < MergableObjectManager.Instance.countForMerge) { return; }

            GameObject particles = ObjectPool.Instance.GetObjectForType("SplitParticles");
            particles.transform.position = transform.position;

            MergableObject[] newShapes = ShapesForSplit ();

            Vector3 center = transform.position;
            int[] sizes = SplitSizes ();

            Vector3[] vertices = VertexOffsets (center);

            for (int i = 0; i < newShapes.Length; i++) {
                newShapes[i].transform.position = PositionForSplit (vertices[i], center);
                // newTetrahedra[i].GetComponent<Rigidbody> ().velocity = VelocityForSplit (vertices[i]);
                newShapes[i].SetSize (sizes[i], Size);
                newShapes[i].gameObject.SetActive (true);
            }

            if (OnShapeSplit != null) { OnShapeSplit (this); }
        }


        public void SetSize (int newSize) {
            SetSize (newSize, Size);
        }

        public void SetSize (int newSize, int oldSize) {

            if(newSize <= oldSize)
            {
                StartMergeCooldownTimer();
            }
            Size = newSize;
            RegisterBySize(newSize, oldSize);
        }


        void InitializeSize () {
            SetSize (Size, -1);
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
            MergableObjectManager manager = MergableObjectManager.Instance;
            //transform.localScale = manager.minimumScale * (1 + Mathf.Log (newSize, manager.countForMerge)) * Vector3.one;

            transform.localScale = (manager.minimumScale * newSize) * Vector3.one;
        }


        MergableObject[] ShapesForSplit () {
            MergableObject[] newShapes = new MergableObject[MergableObjectManager.Instance.countForMerge];
            newShapes[0] = this;
            for (int i = 1; i < MergableObjectManager.Instance.countForMerge; i++) {
                newShapes[i] = GetShapeFromPool ();
            }

            return newShapes;
        }


        int[] SplitSizes () {
            int[] sizes = new int[MergableObjectManager.Instance.countForMerge];

            int average = Size / MergableObjectManager.Instance.countForMerge;
            int remainder = Size % MergableObjectManager.Instance.countForMerge;

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
            if (!_allBySize.ContainsKey (newSize)) { _allBySize.Add (newSize, new List<MergableObject> ()); }
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


        MergableObject GetShapeFromPool () {
            return Utility.ObjectPool.Instance.GetObjectForType (shapeType.ToString(), true).GetComponent<MergableObject> ();
        }

        public void Explode () {
            MergableObject[] mergableObjects = new MergableObject[Size];
            SetSize (1, -1);

            mergableObjects[0] = this;
            for (int i = 1; i < mergableObjects.Length; i++) {
                // TODO: GET FROM POOL!!
                // mergableObjects[i] = GetShapeFromPool ();
                mergableObjects[i] = Instantiate (this);
            }

            foreach (MergableObject mergableObject in mergableObjects) {
                mergableObject.transform.position = transform.position;
                mergableObject.enabled = false;
                mergableObject.gameObject.SetActive (true);
            }
        }

        public ShapeType GetShapeType()
        {
            return shapeType;
        }

        public void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("ShapeMergeRadius"))
            {
                GameObject otherShape = other.gameObject.transform.parent.gameObject;
                otherShape.GetComponent<NotifyOnDisable>().OnObjectDisabled += OnOverlappedDisabled;
                OverlappingMergeTriggers.Add(otherShape.GetComponent<MergableObject>());
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("ShapeMergeRadius"))
            {
                GameObject otherShape = other.gameObject.transform.parent.gameObject;
                otherShape.GetComponent<NotifyOnDisable>().OnObjectDisabled += OnOverlappedDisabled;
                OverlappingMergeTriggers.Remove(otherShape.GetComponent<MergableObject>());
            }
        }

        public void OnOverlappedDisabled(GameObject other)
        {
            OverlappingMergeTriggers.Remove(other.gameObject.GetComponent<MergableObject>());
        }

        #endregion
    }
}
