using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TripleBladeHorse.VFX
{
    public class CopyPlayerMesh : MonoBehaviour
    {
        [SerializeField]
        GameObject partOfPlayer;
        [SerializeField]
        Material customShaderMaterial;
        Transform[] armatureNameChildObj;
        MeshRenderer[] childMeshRenderers;
        Color baseColor;

        public float velocityOfTransparent;

        public void SetEchoMesh(GameObject armature, Color color)
        {
            baseColor = color;
            armatureNameChildObj = armature.GetComponentsInChildren<Transform>();
            List<MeshRenderer> renderers = new List<MeshRenderer>();
            this.transform.localScale = armature.transform.lossyScale;

            for (int i = 0; i < armatureNameChildObj.Length; i++)
            {
                if (armatureNameChildObj[i].GetComponent<MeshRenderer>() != null)
                {
                    GameObject instance = Instantiate(partOfPlayer);
                    instance.transform.parent = this.gameObject.transform;
                    instance.transform.name = armatureNameChildObj[i].name;
                    Transform transformVc = armatureNameChildObj[i].transform;
                    instance.transform.localPosition = transformVc.localPosition;
                    instance.transform.localEulerAngles = transformVc.localEulerAngles;
                    instance.transform.localScale = transformVc.localScale;
                    instance.transform.localRotation = transformVc.localRotation;

                    instance.AddComponent<MeshFilter>();
                    renderers.Add(instance.GetComponent<MeshRenderer>());
                    renderers.Last().material = customShaderMaterial;
                    renderers.Last().material.color = baseColor;
                    var mesh = new Mesh();
                    CopyMesh(ref mesh, armatureNameChildObj[i].GetComponent<MeshFilter>().sharedMesh);
                    instance.GetComponent<MeshFilter>().mesh = mesh;
                }
            }

            childMeshRenderers = renderers.ToArray();
        }

        private void Update()
        {
            foreach (var childObj in childMeshRenderers)
            {
                Color color = childObj.material.color;
                color.a -= velocityOfTransparent * TimeManager.PlayerDeltaTime;
                color.a = Mathf.Clamp01(color.a);
                childObj.material.color = color;
            }

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
}

