using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Utility
{
    public class NotifyOnDisable : MonoBehaviour
    {
        public event System.Action<GameObject> OnObjectDisabled;

        public void OnDisable()
        {
            if (OnObjectDisabled != null)
            {
                OnObjectDisabled(gameObject);
            }
            OnObjectDisabled = null;
        }
    }
}
