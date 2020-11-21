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


        public static Tetrahedron[] All => _all.ToArray ();
        public static int[] Sizes => _sizes.ToArray ();


        private static List<Tetrahedron> _all = new List<Tetrahedron> ();
        private static Dictionary<int, List<Tetrahedron>> _allBySize = new Dictionary<int, List<Tetrahedron>> ();
        private static List<int> _sizes = new List<int> ();


        public static Tetrahedron[] AllBySize (int size) {
            if (!_allBySize.ContainsKey (size) || _allBySize[size].Count == 0) { return new Tetrahedron[0]; }
            return _allBySize[size].ToArray ();
        }
#endregion


#region Instance members

        public ObjectSize SizeComponent {
            get {
                if (_sizeComponent == null) { _sizeComponent = GetComponent<ObjectSize> (); }
                return _sizeComponent;
            }
        }


        private ObjectSize _sizeComponent;


        void OnEnable () {
            _all.Add (this);
            SetSize (GetComponent<ObjectSize> ().Size);
            Game.GameManager.Instance?.OnObjectAdded (this.gameObject);
        }


        void OnDisable () {
            _all.Remove (this);
            Game.GameManager.Instance?.OnObjectRemoved (this.gameObject);
        }


        public void SetSize (int newSize) {
            int oldSize = SizeComponent.Size;
            SizeComponent.Size = newSize;

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
#endregion
    }
}
