using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap_Camera : MonoBehaviour
{
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
    }
}
