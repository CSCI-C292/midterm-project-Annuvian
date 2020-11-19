using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmTree : MonoBehaviour
{
    // References
    public Mesh[] variants;
    public int minSize;
    public int maxSize;
    // Start is called before the first frame update
    void Start()
    {
        int size = Random.Range(minSize, maxSize + 1);
        int variant = Random.Range(0, variants.Length);
        int rotation = Random.Range(0, 360);
        transform.localScale = new Vector3(size, size, size);
        GetComponent<MeshFilter>().mesh = variants[variant];
        transform.localEulerAngles = new Vector3(0, rotation, 0);
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}