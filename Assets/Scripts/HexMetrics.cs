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
        private readonly static Vector3[] corners =
        {
            new Vector3(0f, 0f, outerRadius),
            new Vector3(innerRadius, 0f, outerRadius * 0.5f),
            new Vector3(innerRadius, 0f, outerRadius * -0.5f),
            new Vector3(0f, 0f, -outerRadius),
            new Vector3(-innerRadius, 0f, outerRadius * -0.5f),
            new Vector3(-innerRadius, 0f, outerRadius * 0.5f),
            new Vector3(0f, 0f, outerRadius),
        };

        /// <summary>
        /// Solid factor of the hex cell
        /// </summary>
        public const float solidFactor = 0.75f;

        /// <summary>
        /// Blend factor of the hex cell, it equals 1 - solidFactor
        /// </summary>
        public const float blendFactor = 1f - solidFactor;

        /// <summary>
        /// Get first corner at specified direction
        /// </summary>
        /// <param name="direction">corner direction</param>
        /// <returns>the corner at specified direction</returns>
        public static Vector3 GetFirstCorner(HexDirection direction)
        {
            return corners[(int)direction];
        }

        /// <summary>
        /// Get second corner at specified direction(clockwise direction)
        /// </summary>
        /// <param name="direction">specified direction</param>
        /// <returns>the second corner at specified direction</returns>
        public static Vector3 GetSecondCorner(HexDirection direction)
        {
            return corners[(int)direction + 1];
        }

        /// <summary>
        /// Get first solid corner at specified direction
        /// </summary>
        /// <param name="direction">corner direction</param>
        /// <returns>the solid corner at specified direction</returns>
        public static Vector3 GetFirstSolidCorner(HexDirection direction)
        {
            return corners[(int)direction] * solidFactor;
        }

        /// <summary>
        /// Get second solid corner at specified direction(clockwise direction)
        /// </summary>
        /// <param name="direction">specified direction</param>
        /// <returns>the second solid corner at specified direction</returns>
        public static Vector3 GetSecondSolidCorner(HexDirection direction)
        {
            return corners[(int)direction + 1] * solidFactor;
        }

        /// <summary>
        /// Get bridge offset for edge rectangle at specified direction
        /// </summary>
        /// <param name="direction">edge direction</param>
        /// <returns>bridge offset at specified direction</returns>
        public static Vector3 GetBridge(HexDirection direction)
        {
            return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
        }
    }
}
