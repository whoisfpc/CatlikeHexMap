using UnityEngine;
using System.Collections.Generic;
using System;

namespace HexMap
{
    /// <summary>
    /// The component to generate hexagon map mesh for render
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {
        public bool useCollider;
        public bool useColors;
        public bool useUVCoordinates;
        public bool useUV2Coordinates;

        private Mesh hexMesh;
        private MeshCollider meshCollider;
        [NonSerialized]private List<Vector3> vertices;
        [NonSerialized]private List<int> triangles;
        [NonSerialized]private List<Color> colors;
        [NonSerialized]private List<Vector2> uvs;
        [NonSerialized] private List<Vector2> uv2s;

        private void Awake()
        {
            GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
            if (useCollider)
            {
                meshCollider = gameObject.AddComponent<MeshCollider>();
            }
            hexMesh.name = "Hex Mesh";
        }

        /// <summary>
        /// Clear mesh data
        /// </summary>
        public void Clear()
        {
            hexMesh.Clear();
            vertices = ListPool<Vector3>.Get();
            if (useColors)
            {
                colors = ListPool<Color>.Get();
            }
            if (useUVCoordinates)
            {
                uvs = ListPool<Vector2>.Get();
            }
            if (useUV2Coordinates)
            {
                uv2s = ListPool<Vector2>.Get();
            }
            triangles = ListPool<int>.Get();
        }

        /// <summary>
        /// Apply mesh data
        /// </summary>
        public void Apply()
        {
            hexMesh.SetVertices(vertices);
            ListPool<Vector3>.Add(vertices);
            if (useColors)
            {
                hexMesh.SetColors(colors);
                ListPool<Color>.Add(colors);
            }
            if (useUVCoordinates)
            {
                hexMesh.SetUVs(0, uvs);
                ListPool<Vector2>.Add(uvs);
            }
            if (useUV2Coordinates)
            {
                hexMesh.SetUVs(1, uv2s);
                ListPool<Vector2>.Add(uv2s);
            }
            hexMesh.SetTriangles(triangles, 0);
            ListPool<int>.Add(triangles);
            hexMesh.RecalculateNormals();
            if (useCollider)
            {
                meshCollider.sharedMesh = hexMesh;
            }
        }

        /// <summary>
        /// Add a triangle to hex map mesh
        /// </summary>
        /// <param name="v1">First triangle vertex</param>
        /// <param name="v2">Second triangle vertex</param>
        /// <param name="v3">Third triangle vertex</param>
        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(HexMetrics.Perturb(v1));
            vertices.Add(HexMetrics.Perturb(v2));
            vertices.Add(HexMetrics.Perturb(v3));
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        /// <summary>
        /// Add a triangle to hex map mesh without perturb
        /// </summary>
        /// <param name="v1">First triangle vertex</param>
        /// <param name="v2">Second triangle vertex</param>
        /// <param name="v3">Third triangle vertex</param>
        public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
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
        /// <param name="color">vertex color</param>
        public void AddTriangleColor(Color color)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        /// <summary>
        /// Add vertex color for a triangle
        /// </summary>
        /// <param name="c1">first vertex color</param>
        /// <param name="c2">first vertex color</param>
        /// <param name="c3">first vertex color</param>
        public void AddTriangleColor(Color c1, Color c2, Color c3)
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
        public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(HexMetrics.Perturb(v1));
            vertices.Add(HexMetrics.Perturb(v2));
            vertices.Add(HexMetrics.Perturb(v3));
            vertices.Add(HexMetrics.Perturb(v4));
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
        }

        /// <summary>
        /// Add a quad to hex map mesh without perturb
        /// </summary>
        /// <param name="v1">First triangle vertex</param>
        /// <param name="v2">Second triangle vertex</param>
        /// <param name="v3">Third triangle vertex</param>
        /// <param name="v4">Fourth triangle vertex</param>
        public void AddQuadUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
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
        /// <param name="color">first vertex color</param>
        public void AddQuadColor(Color color)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        /// <summary>
        /// Add vertex color for a quad
        /// </summary>
        /// <param name="c1">first vertex color</param>
        /// <param name="c2">second vertex color</param>
        public void AddQuadColor(Color c1, Color c2)
        {
            colors.Add(c1);
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c2);
        }

        /// <summary>
        /// Add vertex color for a quad
        /// </summary>
        /// <param name="c1">first vertex color</param>
        /// <param name="c2">second vertex color</param>
        /// <param name="c3">third vertex color</param>
        /// <param name="c4">fourth vertex color</param>
        public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
            colors.Add(c4);
        }

        /// <summary>
        /// Add uv for a triangle
        /// </summary>
        /// <param name="uv1">first uv coordinates</param>
        /// <param name="uv2">second uv coordinates</param>
        /// <param name="uv3">third uv coordinates</param>
        public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
        {
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);
        }

        /// <summary>
        /// Add uv for a quad
        /// </summary>
        /// <param name="uv1">first uv coordinates</param>
        /// <param name="uv2">second uv coordinates</param>
        /// <param name="uv3">third uv coordinates</param>
        /// <param name="uv4">fourth uv coordinates</param>
        public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
        {
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);
            uvs.Add(uv4);
        }

        /// <summary>
        /// Add uv for a rectangular quad
        /// </summary>
        /// <param name="uMin">min coordinates of u axis</param>
        /// <param name="uMax">max coordinates of u axis</param>
        /// <param name="vMin">min coordinates of v axis</param>
        /// <param name="vMax">max coordinates of v axis</param>
        public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
        {
            uvs.Add(new Vector2(uMin, vMin));
            uvs.Add(new Vector2(uMax, vMin));
            uvs.Add(new Vector2(uMin, vMax));
            uvs.Add(new Vector2(uMax, vMax));
        }

        /// <summary>
        /// Add uv2 for a triangle
        /// </summary>
        /// <param name="uv1">first uv coordinates</param>
        /// <param name="uv2">second uv coordinates</param>
        /// <param name="uv3">third uv coordinates</param>
        public void AddTriangleUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3)
        {
            uv2s.Add(uv1);
            uv2s.Add(uv2);
            uv2s.Add(uv3);
        }

        /// <summary>
        /// Add uv2 for a quad
        /// </summary>
        /// <param name="uv1">first uv coordinates</param>
        /// <param name="uv2">second uv coordinates</param>
        /// <param name="uv3">third uv coordinates</param>
        /// <param name="uv4">fourth uv coordinates</param>
        public void AddQuadUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
        {
            uv2s.Add(uv1);
            uv2s.Add(uv2);
            uv2s.Add(uv3);
            uv2s.Add(uv4);
        }

        /// <summary>
        /// Add uv2 for a rectangular quad
        /// </summary>
        /// <param name="uMin">min coordinates of u axis</param>
        /// <param name="uMax">max coordinates of u axis</param>
        /// <param name="vMin">min coordinates of v axis</param>
        /// <param name="vMax">max coordinates of v axis</param>
        public void AddQuadUV2(float uMin, float uMax, float vMin, float vMax)
        {
            uv2s.Add(new Vector2(uMin, vMin));
            uv2s.Add(new Vector2(uMax, vMin));
            uv2s.Add(new Vector2(uMin, vMax));
            uv2s.Add(new Vector2(uMax, vMax));
        }
    }
}
