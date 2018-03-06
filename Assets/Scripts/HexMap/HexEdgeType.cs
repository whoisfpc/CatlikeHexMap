namespace HexMap
{
    /// <summary>
    /// Enumeration for hex edge connect type
    /// </summary>
    public enum HexEdgeType
    {
        /// <summary>
        /// Flat edge type, two hex cells have same elevation
        /// </summary>
        Flat,
        /// <summary>
        /// Slope edge type, with terrace
        /// </summary>
        Slope,
        /// <summary>
        /// Cliff edge type, without terrace
        /// </summary>
        Cliff
    }
}
