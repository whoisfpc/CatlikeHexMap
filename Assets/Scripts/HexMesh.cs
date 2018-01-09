using UnityEngine;
using System.Collections.Generic;

namespace HexMap
{
    /// <summary>
    /// The component to generate hexagon map mesh for render
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {
        private Mesh hexMesh;
        private MeshCollider meshCollider;
        private List<Vector3> vertices;
        private List<int> triangles;
        private List<Color> colors;

        private void Awake()
        {
            GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
            meshCollider = gameObject.AddComponent<MeshCollider>(); 
            hexMesh.name = "Hex Mesh";
            vertices = new List<Vector3>();
            triangles = new List<int>();
            colors = new List<Color>();
        }

        /// <summary>
        /// Generate hexagon map mesh according to specified hex cells properties
        /// </summary>
        /// <param name="cells">hex cells for generating hex map</param>
        public void Triangulate(HexCell[] cells)
        {
            hexMesh.Clear();
            vertices.Clear();
            colors.Clear();
            triangles.Clear();

            for (int i = 0; i < cells.Length; i++)
            {
                Triangulate(cells[i]);
            }

            hexMesh.vertices = vertices.ToArray();
            hexMesh.colors = colors.ToArray();
            hexMesh.triangles = triangles.ToArray();
            hexMesh.RecalculateNormals();
            meshCollider.sharedMesh = hexMesh;
        }

        /// <summary>
        /// Add a triangle and corresponding vertex color according to specified hex cell
        /// </summary>
        /// <param name="cell">specified hex cell</param>
        private void Triangulate(HexCell cell)
        {
            var center = cell.transform.localPosition;
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(center, center + HexMetrics.corners[i], center + HexMetrics.corners[i + 1]);
                AddTriangleColor(cell.color);
            }
        }

        /// <summary>
        /// Add a triangle to hex map mesh
        /// </summary>
        /// <param name="v1">First triangle vertex</param>
        /// <param name="v2">Second triangle vertex</param>
        /// <param name="v3">Third triangle vertex</param>
        private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        /// <summary>
        /// Add vertex color for a triangle
        /// </summary>
        /// <param name="color">specified vertex color</param>
        private void AddTriangleColor(Color color)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }
    }
}
