/*
* Used when gameobjects has component that needs to do something
* onAwake() method
*
*/


using System.Collections.Generic;
using UnityEngine;

public class NodeStabilizer : MonoBehaviour
{

    public List<GameObject> SetActiveAtLoad = new List<GameObject>();
    public List<GameObject> SetInactiveAtLoad = new List<GameObject>();

    void Awake()
    {
        foreach (GameObject item in SetActiveAtLoad)
        {
#if UNITY_EDITOR
            if (item == null)
            {
                Debug.LogWarning("NodeStabilizer Warning: There is null object in list in " + gameObject.name);
            }
#endif
            if (item != null) item.SetActive(true);

        }

        foreach (GameObject item in SetInactiveAtLoad)
        {
#if UNITY_EDITOR
            if (item == null)
            {
                Debug.LogWarning("NodeStabilizer Warning: There is null object in list");
            }
#endif
            if (item != null) item.SetActive(false);
        }

        Destroy(this);
    }
}
