using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisions : MonoBehaviour
{
    public List<GameObject> ignoreColliders = new List<GameObject>();

    void Start()
    {
        foreach(GameObject collider in ignoreColliders){
            Physics.IgnoreCollision(this.gameObject.GetComponent<MeshCollider>(), collider.gameObject.GetComponent<MeshCollider>(), true);
        }
    }

}
