namespace DavidStudio.Core.Essentials.CompleteSample.Data;

public static class Permissions
{
    public static class Products
    {
        public const string Read = $"{nameof(Products)}.{nameof(Read)}";
        public const string Manage = $"{nameof(Products)}.{nameof(Manage)}";
    }
    
    public static class Manufacturers
    {
        public const string Read = $"{nameof(Manufacturers)}.{nameof(Read)}";
        public const string Manage = $"{nameof(Manufacturers)}.{nameof(Manage)}";
    }
}