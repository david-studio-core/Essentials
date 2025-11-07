namespace DavidStudio.Core.Utilities.Utilities;

public static class CommonUtilities
{
    public static int CalculateAge(this DateTime birthDate, DateTime currentDate = default)
    {
        if (currentDate == default) currentDate = DateTime.UtcNow;

        var age = currentDate.Year - birthDate.Year;
        if (birthDate.Date > currentDate.AddYears(-age)) age--;
        return age;
    }

    public static int DetermineGeohashPrecision(
        double topLeftLat, double bottomRightLat, double topLeftLon, double bottomRightLon)
    {
        var latDiff = Math.Abs(topLeftLat - bottomRightLat);
        var lonDiff = Math.Abs(topLeftLon - bottomRightLon);

        // Estimate precision based on box size
        if (latDiff > 10 || lonDiff > 10) return 3; // Country-level
        if (latDiff > 5 || lonDiff > 5) return 4; // Regional level
        if (latDiff > 1 || lonDiff > 1) return 5; // City-level
        if (latDiff > 0.1 || lonDiff > 0.1) return 6; // Neighborhood-level
        return 7; // Street-level detail
    }

    public static decimal Map(this decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

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