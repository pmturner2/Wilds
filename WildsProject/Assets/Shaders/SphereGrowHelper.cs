using UnityEngine;
using System.Collections;

public class SphereGrowHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MeshFilter mf = GetComponent(typeof(MeshFilter)) as MeshFilter;
        Mesh m = mf.mesh;
        Vector3 avg = Vector3.zero;
        for (int i = 0; i < m.vertexCount; i++)
        {
            avg += m.vertices[i];
        }
        avg /= m.vertexCount;
        renderer.material.SetVector("_Center", avg);
	}

    // Update is called once per frame
    void Update()
    {
	
	}
}
