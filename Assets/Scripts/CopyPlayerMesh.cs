using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPlayerMesh : MonoBehaviour
{
    [SerializeField]
    GameObject partOfPlayer;
    [SerializeField]
    GameObject armatureNameObj;

    Transform[] armatureNameChildObj;
    Transform[] childGameObjectsTrans;

    int hash;
    MeshFilter testObj;

    public float velocityOfTransparent;

    private void Start()
    {
        armatureNameObj = GameObject.Find("armatureName");
        armatureNameChildObj = armatureNameObj.GetComponentsInChildren<Transform>();
        for (int i = 0; i < armatureNameChildObj.Length; i++)
        {
            if (armatureNameChildObj[i].name != "armatureName" && armatureNameChildObj[i].name != "Sword")
            {
                GameObject instance = (GameObject)Instantiate(partOfPlayer);
                instance.transform.parent = this.gameObject.transform;
                instance.transform.name = armatureNameChildObj[i].name;
                Transform transformVc = armatureNameChildObj[i].transform;
                instance.transform.localPosition = transformVc.localPosition;
                instance.transform.localEulerAngles = transformVc.localEulerAngles;
                instance.transform.localScale = transformVc.localScale;
                instance.transform.localRotation = transformVc.localRotation;
                //instance.AddComponent<MeshRenderer>();
                instance.AddComponent<MeshFilter>();
                instance.GetComponent<MeshRenderer>().material =
                        armatureNameChildObj[i].GetComponent<MeshRenderer>().material;
                var mesh = new Mesh();
                CopyMesh(ref mesh, armatureNameChildObj[i].GetComponent<MeshFilter>().sharedMesh);
                instance.GetComponent<MeshFilter>().mesh = mesh;
            }
        }

        testObj = armatureNameChildObj[3].GetComponent<MeshFilter>();
        hash = testObj.mesh.GetHashCode();
        print("Hash Test " + hash);
    }

    private void Update()
    {
        childGameObjectsTrans = this.transform.GetComponentsInChildren<Transform>();
        foreach(var childObj in childGameObjectsTrans)
        {
            if(childObj.GetComponent<MeshRenderer>() != null)
            {
                Color color = childObj.GetComponent<MeshRenderer>().material.color;
                color.a = Mathf.Lerp(color.a, 0, velocityOfTransparent * Time.deltaTime);
                childObj.GetComponent<MeshRenderer>().material.color = color;
            }
        }

        print("Hash " + hash);
    }

    private void CopyMesh(ref Mesh output, Mesh input)
    {
        List<Vector3> vertices = new List<Vector3>();
        foreach (var item in input.vertices)
        {
            vertices.Add(item);
        }
        output.SetVertices(vertices);

        List<int> triangles = new List<int>();
        foreach (var item in input.triangles)
        {
            triangles.Add(item);
        }
        output.SetTriangles(triangles, 0);

        List<int> indices = new List<int>();
        var inputIndices = input.GetIndices(0);
        foreach (var item in inputIndices)
        {
            indices.Add(item);
        }
        output.SetIndices(indices.ToArray(), input.GetTopology(0), 0);

        List<Vector3> normals = new List<Vector3>();
        foreach (var item in input.normals)
        {
            normals.Add(item);
        }
        output.SetNormals(normals);

        List<Vector2> uv = new List<Vector2>();
        foreach (var item in input.uv)
        {
            uv.Add(item);
        }
        output.SetUVs(0, uv);
    }
}
