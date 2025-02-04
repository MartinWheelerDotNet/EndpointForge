// In EndpointForge.Attributes/StringEnumeratorAttribute.cs (or similar)

using System;

namespace EndpointForge.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)] // Apply to classes
    public class StringEnumeratorAttribute: Attribute { }
}