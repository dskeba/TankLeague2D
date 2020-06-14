using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        public Dictionary<string, Queue<GameObject>> freeObjects = new Dictionary<string, Queue<GameObject>>();
        public Dictionary<string, List<GameObject>> usedObjects = new Dictionary<string, List<GameObject>>();

        private Dictionary<string, bool> poolLock = new Dictionary<string, bool>();
        private bool expandWhenEmpty;

        public void Load(string prefabPath, int size = 4, bool expandWhenEmpty = true)
        {
            this.expandWhenEmpty = expandWhenEmpty;
            if (!freeObjects.ContainsKey(prefabPath))
            {
                freeObjects.Add(prefabPath, new Queue<GameObject>());
                usedObjects.Add(prefabPath, new List<GameObject>());
            }
            for (int i = 0; i < size; i++)
            {
                InstantiatePoolObject(prefabPath);
            }
            poolLock[prefabPath] = false;
        }

        public void Unload(string prefabPath)
        {
            poolLock[prefabPath] = true;
            while (freeObjects[prefabPath].Count > 0)
            {
                var gameObject = freeObjects[prefabPath].Dequeue();
                Destroy(gameObject);
            }
            for (int i = usedObjects[prefabPath].Count - 1; i >= 0; i--)
            {
                var gameObject = usedObjects[prefabPath][i];
                usedObjects[prefabPath].RemoveAt(i);
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }

        private GameObject InstantiatePoolObject(string prefabPath)
        {
            var prefab = Resources.Load<GameObject>(prefabPath);
            var instance = Instantiate(prefab);
            instance.SetActive(false);
            DontDestroyOnLoad(instance);
            freeObjects[prefabPath].Enqueue(instance);
            return instance;
        }

        public GameObject Get(string prefabPath)
        {
            if (poolLock[prefabPath])
            {
                return null;
            }
            if (freeObjects[prefabPath].Count == 0)
            {
                if (!expandWhenEmpty)
                {
                    return null;
                }
                else
                {
                    InstantiatePoolObject(prefabPath);
                }
            }
            var gameObject = freeObjects[prefabPath].Dequeue();
            usedObjects[prefabPath].Add(gameObject);
            return gameObject;
        }

        public void Return(string prefabPath, GameObject gameObject)
        {
            if (poolLock[prefabPath])
            {
                return;
            }
            gameObject.SetActive(false);
            usedObjects[prefabPath].Remove(gameObject);
            freeObjects[prefabPath].Enqueue(gameObject);
        }

        public void Return(string prefabPath, GameObject gameObject, float secondsToWait)
        {
            if (poolLock[prefabPath])
            {
                return;
            }
            StartCoroutine(ReturnAfterSecondsToWait(prefabPath, gameObject, secondsToWait));
        }

        public IEnumerator ReturnAfterSecondsToWait(string prefabPath, GameObject gameObject, float secondsToWait)
        {
            yield return new WaitForSeconds(secondsToWait);
            Return(prefabPath, gameObject);
        }
    }
}
