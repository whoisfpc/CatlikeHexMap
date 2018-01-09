using UnityEngine;

namespace HexMap
{
    /// <summary>
    /// Hexagon cell's metrics
    /// </summary>
    public static class HexMetrics
    {
        /// <summary>
        /// Radius of the outer circle of hex cell
        /// </summary>
        public const float outerRadius = 10f;
        
        /// <summary>
        /// Radius of the inner circle of hex cell
        /// </summary>
        public const float innerRadius = outerRadius * 0.866025404f;
        
        /// <summary>
        /// Corners of the hex cell, clockwise direction
        /// </summary>
        public readonly static Vector3[] corners =
        {
            new Vector3(0f, 0f, outerRadius),
            new Vector3(innerRadius, 0f, outerRadius * 0.5f),
            new Vector3(innerRadius, 0f, outerRadius * -0.5f),
            new Vector3(0f, 0f, -outerRadius),
            new Vector3(-innerRadius, 0f, outerRadius * -0.5f),
            new Vector3(-innerRadius, 0f, outerRadius * 0.5f),
            new Vector3(0f, 0f, outerRadius),
       };
    }
}
