using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Utility
{
    public class ObjectPool : MonoBehaviour
    {

        public static ObjectPool Instance
        {
            get; private set;
        }

        /// <summary>
        /// The pooled objects currently available.
        /// </summary>
        private List<GameObject>[] pooledObjects;

        [System.Serializable]
        public struct PoolableObject
        {
            public int numberToPool;

            public GameObject prefab;
        }

        public PoolableObject[] poolableObjects;

        void Awake()
        {
            Instance = this;

            //Loop through the object prefabs and make a new list for each one.
            //We do this because the pool can only support prefabs set to it in the editor,
            //so we can assume the lists of pooled objects are in the same order as object prefabs in the array
            pooledObjects = new List<GameObject>[poolableObjects.Length];

            int i = 0;
            foreach (PoolableObject poolable in poolableObjects)
            {
                pooledObjects[i] = new List<GameObject>();

                for (int n = 0; n < poolable.numberToPool; n++)
                {
                    GameObject newObj = Instantiate(poolable.prefab) as GameObject;
                    newObj.name = poolable.prefab.name;
                    PoolObject(newObj);
                }

                i++;
            }
        }

        /// <summary>
        /// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
        /// then null will be returned.
        /// </summary>
        /// <returns>
        /// The object for type.
        /// </returns>
        /// <param name='objectType'>
        /// Object type.
        /// </param>
        /// <param name='onlyPooled'>
        /// If true, it will only return an object if there is one currently pooled.
        /// </param>
        public GameObject GetObjectForType(string objectType, bool onlyPooled = false)
        {
            //Debug.Log("Getting " + objectType);
            GameObject obj = null;
            for (int i = 0; i < poolableObjects.Length; i++)
            {
                GameObject prefab = poolableObjects[i].prefab;
                if (prefab.name == objectType)
                {
                    //Debug.Log("Pooled objects of type " + objectType + ": " + pooledObjects[i].Count);
                    if (pooledObjects[i].Count > 0)
                    {
                        obj = pooledObjects[i][0];
                        pooledObjects[i].RemoveAt(0);
                        obj.SetActive(true);
                        obj.transform.SetParent(null);
                    }
                    else if (!onlyPooled)
                    {
                        //Debug.Log("Not enough pooled of type " + objectType + ".  Instantiating.");
                        obj = Instantiate(poolableObjects[i].prefab) as GameObject;
                        // Must have the correct name so it can be pooled.
                        obj.name = poolableObjects[i].prefab.name;
                        obj.transform.SetParent(this.transform);
                        obj.SetActive(true);
                        obj.transform.SetParent(null);
                    }
                    SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
                    break;
                }
            }

            return obj;
        }

        /// <summary>
        /// Pools the object specified.  Will not be pooled if there is no prefab of that type.
        /// </summary>
        /// <param name='obj'>
        /// Object to be pooled.
        /// </param>
        public void PoolObject(GameObject obj)
        {
            //Debug.Log("PoolObject");
            //Debug.Log("obj.name=" + obj.name);
            if (obj == null)
            {
                Debug.Log("Trying to pool a null object.  Was it destroyed?");
            }
            else
            {
                for (int i = 0; i < poolableObjects.Length; i++)
                {
                    if (poolableObjects[i].prefab.name == obj.name)
                    {
                        obj.SetActive(false);
                        obj.transform.SetParent(transform);
                        obj.transform.localPosition = new Vector3();
                        //Debug.Log("Adding to list");
                        pooledObjects[i].Add(obj);
                        //Debug.Log("List size is " + pooledObjects[i].Count);
                        return;
                    }
                }
            }

            //Debug.Log("Attempt to pool object " + obj.name + " but no pool was found.");
        }

        public void PoolObject(GameObject obj, float delay)
        {
            StartCoroutine(PoolObjectDelayed(obj, delay));
        }

        private IEnumerator PoolObjectDelayed(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            PoolObject(obj);
        }

    }
}
