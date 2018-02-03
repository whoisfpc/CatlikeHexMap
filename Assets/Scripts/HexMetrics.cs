using UnityEngine;

namespace HexMap
{
    /// <summary>
    /// Hexagon cell's metrics, contains properties for Hexagon cells and hex map
    /// </summary>
    public static class HexMetrics
    {
        /// <summary>
        /// Stream bed elevation offset
        /// </summary>
        public const float streamBedElevationOffset = -1.75f;

        /// <summary>
        /// Water surface elevation offset
        /// </summary>
        public const float waterElevationOffset = -0.5f;

        public const float waterFactor = 0.6f;

        public const float waterBlendFactor = 1f - waterFactor;

        public static Vector3 GetFirstWaterCorner(HexDirection direction)
        {
            return corners[(int)direction] * waterFactor;
        }

        public static Vector3 GetSecondWaterCorner(HexDirection direction)
        {
            return corners[(int)direction + 1] * waterFactor;
        }

        public static Vector3 GetWaterBridge(HexDirection direction)
        {
            return (corners[(int)direction] + corners[(int)direction + 1]) * waterBlendFactor;
        }

        /// <summary>
        /// cell size per chunk
        /// </summary>
        public const int chunkSizeX = 5, chunkSizeZ = 5;

        public const float outerToInner = 0.866025404f;

        public const float innerToOuter = 1f / outerToInner;

        /// <summary>
        /// Radius of the outer circle of hex cell
        /// </summary>
        public const float outerRadius = 10f;
        
        /// <summary>
        /// Radius of the inner circle of hex cell
        /// </summary>
        public const float innerRadius = outerRadius * outerToInner;
        
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
        public const float solidFactor = 0.8f;

        /// <summary>
        /// Blend factor of the hex cell, it equals 1 - solidFactor
        /// </summary>
        public const float blendFactor = 1f - solidFactor;

        /// <summary>
        /// the height between each successive elevation step
        /// </summary>
        public const float elevationStep = 3f;

        /// <summary>
        /// Number of terraces per slope
        /// </summary>
        public const int terracesPerSlope = 2;

        /// <summary>
        /// Number of terrace steps per slope, it equals terracesPerSlope * 2 + 1
        /// </summary>
        public const int terraceSteps = terracesPerSlope * 2 + 1;

        /// <summary>
        /// Size of horizontal terrace step
        /// </summary>
        public const float horizontalTerraceStepSize = 1f / terraceSteps;

        /// <summary>
        /// Size of vertical terrace step
        /// </summary>
        public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

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

        /// <summary>
        /// A special lerp for terrace steps
        /// </summary>
        /// <param name="a">start lerp point</param>
        /// <param name="b">end lerp point</param>
        /// <param name="step">lerp step</param>
        /// <returns>corresponding point at step</returns>
        public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
        {
            var h = step * horizontalTerraceStepSize;
            a.x += (b.x - a.x) * h;
            a.z += (b.z - a.z) * h;
            var v = ((step + 1) / 2) * verticalTerraceStepSize;
            a.y += (b.y - a.y) * v;
            return a;
        }

        /// <summary>
        /// Lerp for terrace slope's color
        /// </summary>
        /// <param name="a">start lerp color</param>
        /// <param name="b">end lerp color</param>
        /// <param name="step">lerp step</param>
        /// <returns>corresponding color at step</returns>
        public static Color TerraceLerp(Color a, Color b, int step)
        {
            float h = step * horizontalTerraceStepSize;
            return Color.Lerp(a, b, h);
        }

        /// <summary>
        /// Get the HexEdgeType by two specified elevations
        /// </summary>
        /// <param name="elevation1">elevation 1</param>
        /// <param name="elevation2">elevation 2</param>
        /// <returns>HexEdgeType for two elevations</returns>
        public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
        {
            if (elevation1 == elevation2)
            {
                return HexEdgeType.Flat;
            }

            var delta = elevation1 - elevation2;
            if (delta == 1 || delta == -1)
            {
                return HexEdgeType.Slope;
            }

            return HexEdgeType.Cliff;
        }

        public static Vector3 GetSolidEdgeMiddle(HexDirection direction)
        {
            return (corners[(int)direction] + corners[(int)direction + 1]) * (0.5f * solidFactor);
        }

        /// <summary>
        /// Noise texture for irregularity
        /// </summary>
        public static Texture2D noiseSource;

        /// <summary>
        /// Perturb strength
        /// </summary>
        public const float cellPerturbStrength = 4f;

        /// <summary>
        /// Perturb strength for elevation
        /// </summary>
        public const float elevationPerturbStrength = 1.5f;

        /// <summary>
        /// Noise sample scale
        /// </summary>
        public const float noiseScale = 0.003f;

        /// <summary>
        /// Sample noise color at position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>bilinear filtered color at position</returns>
        public static Vector4 SampleNoise(Vector3 position)
        {
            return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
        }

        /// <summary>
        /// Perturb (x,z) position according to noise texture
        /// </summary>
        /// <param name="position">position to perturb</param>
        /// <returns>position after perturbed</returns>
        public static Vector3 Perturb(Vector3 position)
        {
            Vector4 sample = SampleNoise(position);
            position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
            position.z += (sample.z * 2f - 1f) * cellPerturbStrength;
            return position;
        }
    }
}
