using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class ObjectPoolItem
    {
        public GameObject objectToPool;
        public int amountToPool;
    }

    public List<ObjectPoolItem> itemsToPool;
    public List<List<GameObject>> pooledObjects;

    void Awake()
    {
        // Create the list of pooled objects for each item
        pooledObjects = new List<List<GameObject>>();
        foreach (var item in itemsToPool)
        {
            List<GameObject> itemPooledObjects = new List<GameObject>();
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = Instantiate(item.objectToPool);
                obj.SetActive(false);
                itemPooledObjects.Add(obj);
            }
            pooledObjects.Add(itemPooledObjects);
        }
    }

    public GameObject GetPooledObject(int index)
    {
        var itemPooledObjects = pooledObjects[index];
        for (int i = 0; i < itemPooledObjects.Count; i++)
        {
            if (!itemPooledObjects[i].activeInHierarchy)
            {
                return itemPooledObjects[i];
            }
        }
        return null;
    }
}
