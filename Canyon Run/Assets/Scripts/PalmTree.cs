using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmTree : MonoBehaviour
{
    // References
    public Mesh[] variants;
    public int minSize;
    public int maxSize;

    void Start()
    {
        // Randomizes the size, mesh, and roation of the tree upon spawning
        int size = Random.Range(minSize, maxSize + 1);
        int variant = Random.Range(0, variants.Length);
        int rotation = Random.Range(0, 360);

        // Applies the randomized settings
        transform.localScale = new Vector3(size, size, size);
        GetComponent<MeshFilter>().mesh = variants[variant];
        transform.localEulerAngles = new Vector3(0, rotation, 0);
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
    }
}