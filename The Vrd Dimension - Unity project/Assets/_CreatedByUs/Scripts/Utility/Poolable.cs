using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public class Poolable : MonoBehaviour
    {
        [SerializeField]
        private string poolName;

        public string PoolName
        {
            get
            {
                return poolName;
            }

            private set
            {
                poolName = value;
            }
        }
    }

}
