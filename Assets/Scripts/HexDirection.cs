namespace HexMap
{
    /// <summary>
    /// hex direction enumeration
    /// </summary>
    public enum HexDirection
    {
        /// <summary>
        /// Northeast
        /// </summary>
        NE,
        /// <summary>
        /// East
        /// </summary>
        E,
        /// <summary>
        /// Southeast
        /// </summary>
        SE,
        /// <summary>
        /// Southwest
        /// </summary>
        SW,
        /// <summary>
        /// West
        /// </summary>
        W,
        /// <summary>
        /// Northwest 
        /// </summary>
        NW
    }

    /// <summary>
    /// Extensions methods for HexDirection enumeration
    /// </summary>
    public static class HexDirectionExtensions
    {
        /// <summary>
        /// Obtain opposite direction of specified direction
        /// </summary>
        /// <param name="direction">the direction</param>
        /// <returns>opposite direction of specified direction</returns>
        public static HexDirection Opposite(this HexDirection direction)
        {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }
    }
}
