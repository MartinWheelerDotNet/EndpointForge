namespace EndpointForge.Abstractions.Constants;

/// <summary>
/// Contains types of EndpointForge parameters
/// </summary>
public static partial class ParameterType
{
    // ReSharper disable ConvertToConstant.Global
    // These are deliberately `static readonly` and not `const`.  This is so that all consuming assemblies
    // get the exact same `string` instance and comparisons are optimized.

    /// <summary>
    ///  EndpointForge parameter type "header"
    /// </summary>
    public static readonly string Header = "header";
    
    /// <summary>
    ///  EndpointForge parameter type "static"
    /// </summary>
    public static readonly string Static = "static";

    /// <summary>
    /// Returns a value that indicates if the EndpointForge parameter type is Header.
    /// </summary>
    /// <param name="type">EndpointForge parameter type.</param>
    /// <returns>
    /// <see langword="true" /> if the type is Header; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsHeader(string type) => Equals(Header, type);
    
    /// <summary>
    /// Returns a value that indicates if the EndpointForge parameter type is Static.
    /// </summary>
    /// <param name="type">EndpointForge parameter type.</param>
    /// <returns>
    /// <see langword="true" /> if the type is Static; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsStatic(string type) => Equals(Static, type);

    /// <summary>
    /// Returns a value that indicates if the provided ReadOnlyCharSpan is a member of this string enum.
    /// </summary>
    /// <param name="value">The ReadOnlyCharSpan to be compared.</param>
    /// <returns>
    /// <see langword="true" />if the ReadOnlyCharSpan is a member of this string enum; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsMember(ReadOnlySpan<char> value)
        => value switch
        {
            _ when value.SequenceEqual(Header) => true,
            _ when value.SequenceEqual(Static) => true,
            _ => false
        };
    
    /// <summary>
    /// Returns a value that indicates if the EndpointForge parameter types are the same.
    /// </summary>
    /// <param name="typeA">The first EndpointForge parameter type to compare.</param>
    /// <param name="typeB">The second EndpointForge parameter type to compare.</param>
    /// <returns>
    /// <see langword="true" /> if the types are the same; otherwise, <see langword="false" />.
    /// </returns>
    public static bool Equals(string typeA, string typeB)
    {
        return ReferenceEquals(typeA, typeB) || StringComparer.OrdinalIgnoreCase.Equals(typeA, typeB);
    }
    
    
}