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
        /// Generate a hex cell mesh and set colors according to specified hex cell
        /// </summary>
        /// <param name="cell">specified hex cell</param>
        private void Triangulate(HexCell cell)
        {
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                Triangulate(d, cell);
            }
        }

        /// <summary>
        /// Add a triangle and color for specified hex cell at specified direction
        /// </summary>
        /// <param name="direction">specified direction</param>
        /// <param name="cell">hex cell</param>
        private void Triangulate(HexDirection direction, HexCell cell)
        {
            var center = cell.transform.localPosition;
            var v1 = center + HexMetrics.GetFirstSolidCorner(direction);
            var v2 = center + HexMetrics.GetSecondSolidCorner(direction);
            AddTriangle(center, v1, v2);
            AddTriangleColor(cell.color, cell.color, cell.color);

            if (direction <= HexDirection.SE)
            {
                TriangulateConnection(direction, cell, v1, v2);
            }
        }

        /// <summary>
        /// Deal with connection between with hexagon cells,
        /// including mesh and color
        /// </summary>
        /// <param name="direction">connectin direction</param>
        /// <param name="cell">hex cell</param>
        /// <param name="v1">first solid conrner</param>
        /// <param name="v2">second solid conrner</param>
        private void TriangulateConnection(HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2)
        {
            var neighbor = cell.GetNeighbor(direction);

            if (neighbor == null)
            {
                return;
            }

            var bridge = HexMetrics.GetBridge(direction);
            var v3 = v1 + bridge;
            var v4 = v2 + bridge;

            AddQuad(v1, v2, v3, v4);
            AddQuadColor(cell.color, neighbor.color);

            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.SE && nextNeighbor != null)
            {
                AddTriangle(v2, v4, v2 + HexMetrics.GetBridge(direction.Next()));
                AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
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
        /// <param name="c1">first vertex color</param>
        /// <param name="c2">first vertex color</param>
        /// <param name="c3">first vertex color</param>
        private void AddTriangleColor(Color c1, Color c2, Color c3)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
        }

        /// <summary>
        /// Add a quad to hex map mesh
        /// </summary>
        /// <param name="v1">first quad vertex</param>
        /// <param name="v2">second quad vertex</param>
        /// <param name="v3">third quad vertex</param>
        /// <param name="v4">fourth quad vertex</param>
        private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
        }

        /// <summary>
        /// Add vertex color for a quad
        /// </summary>
        /// <param name="c1">first vertex color</param>
        /// <param name="c2">second vertex color</param>
        private void AddQuadColor(Color c1, Color c2)
        {
            colors.Add(c1);
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c2);
        }
    }
}
