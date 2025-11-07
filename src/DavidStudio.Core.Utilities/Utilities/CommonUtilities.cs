namespace DavidStudio.Core.Utilities.Utilities;

/// <summary>
/// Provides common utilities.
/// </summary>
public static class CommonUtilities
{
    /// <summary>
    /// Calculates the age based on a given birthdate and an optional current date.
    /// </summary>
    /// <param name="birthDate">The birthdate.</param>
    /// <param name="currentDate">
    /// The reference date for age calculation. Defaults to <see cref="DateTime.UtcNow"/> if not provided.
    /// </param>
    /// <returns>The age in full years.</returns>
    /// <remarks>
    /// Subtracts the birth year from the current year and adjusts if the birthday has not yet occurred in the current year.
    /// </remarks>
    public static int CalculateAge(this DateTime birthDate, DateTime currentDate = default)
    {
        if (currentDate == default) currentDate = DateTime.UtcNow;

        var age = currentDate.Year - birthDate.Year;
        if (birthDate.Date > currentDate.AddYears(-age)) age--;
        return age;
    }

    /// <summary>
    /// Determines an appropriate geohash precision level based on the latitude and longitude bounds of a rectangular area.
    /// </summary>
    /// <param name="topLeftLat">Latitude of the top-left corner of the rectangle.</param>
    /// <param name="bottomRightLat">Latitude of the bottom-right corner of the rectangle.</param>
    /// <param name="topLeftLon">Longitude of the top-left corner of the rectangle.</param>
    /// <param name="bottomRightLon">Longitude of the bottom-right corner of the rectangle.</param>
    /// <returns>An integer representing the geohash precision level:
    /// <list type="bullet">
    /// <item>3 – Country-level</item>
    /// <item>4 – Regional-level</item>
    /// <item>5 – City-level</item>
    /// <item>6 – Neighborhood-level</item>
    /// <item>7 – Street-level</item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// The precision is estimated based on the difference between latitude and longitude coordinates.
    /// </remarks>
    public static int DetermineGeohashPrecision(
        double topLeftLat, double bottomRightLat, double topLeftLon, double bottomRightLon)
    {
        var latDiff = Math.Abs(topLeftLat - bottomRightLat);
        var lonDiff = Math.Abs(topLeftLon - bottomRightLon);

        // Estimate precision based on box size
        if (latDiff > 10 || lonDiff > 10) return 3;
        if (latDiff > 5 || lonDiff > 5) return 4;
        if (latDiff > 1 || lonDiff > 1) return 5;
        if (latDiff > 0.1 || lonDiff > 0.1) return 6;
        return 7;
    }

    /// <summary>
    /// Maps a decimal value from a source range to a target range proportionally.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="fromSource">The minimum of the source range.</param>
    /// <param name="toSource">The maximum of the source range.</param>
    /// <param name="fromTarget">The minimum of the target range.</param>
    /// <param name="toTarget">The maximum of the target range.</param>
    /// <returns>The value mapped to the target range.</returns>
    /// <remarks>
    /// Uses the formula: (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget
    /// to scale the value proportionally between ranges.
    /// </remarks>
    public static decimal Map(this decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    /// <summary>
    /// Determines whether an inner range is completely within an outer range.
    /// </summary>
    /// <typeparam name="T">The type of values in the range. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="outerStart">The start of the outer range.</param>
    /// <param name="outerEnd">The end of the outer range.</param>
    /// <param name="innerStart">The start of the inner range.</param>
    /// <param name="innerEnd">The end of the inner range.</param>
    /// <param name="isInclusive">
    /// Whether to include the boundary values in the check. Defaults to <c>true</c>.
    /// </param>
    /// <returns><c>true</c> if the inner range is within the outer range; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// If <paramref name="isInclusive"/> is <c>true</c>, the comparison includes equality at the boundaries.
    /// </remarks>
    public static bool IsInRange<T>(T outerStart, T outerEnd, T innerStart, T innerEnd,
        bool isInclusive = true)
        where T : IComparable<T>
    {
        if (isInclusive)
            return innerStart.CompareTo(outerStart) >= 0 && innerEnd.CompareTo(outerEnd) <= 0;
        else
            return innerStart.CompareTo(outerStart) > 0 && innerEnd.CompareTo(outerEnd) < 0;
    }
}