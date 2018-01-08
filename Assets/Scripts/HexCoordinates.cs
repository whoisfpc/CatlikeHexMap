using UnityEngine;

namespace HexMap
{
    [System.Serializable]
    public struct HexCoordinates
    {
        [SerializeField]
        private int x, z;

        public int X => x;
        public int Z => z;
        public int Y => -X - Z;

        public HexCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public static HexCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexCoordinates(x - z / 2, z);
        }

        public override string ToString() => $"({X}, {Y}, {Z})";

        public string ToStringOnSeparateLines() => $"{X}\n{Y}\n{Z}";
    }
}
