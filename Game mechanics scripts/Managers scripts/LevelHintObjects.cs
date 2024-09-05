using UnityEngine;
using System.Collections.Generic;

public class LevelHintObjects : MonoBehaviour
{
    public List<GameObject> hintObjects;

    void Start()
    {
        if (HintManager.Instance != null)
        {
            HintManager.Instance.SetHintObjects(hintObjects);
        }
    }
}
