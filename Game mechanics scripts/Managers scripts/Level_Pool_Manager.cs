using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Pool_Manager : MonoBehaviour
{
    public List<GameObject> pooledObjects;

    public static Level_Pool_Manager _instance;

    private void Awake()
    {
        _instance = this;
    }

    public void Add_Object(GameObject _item)
    {
        pooledObjects.Add(_item);
    }

    public void Clean_All_Objects()
    {
        foreach (GameObject _child in pooledObjects)
            Destroy(_child);
        pooledObjects.Clear();
    }
}
