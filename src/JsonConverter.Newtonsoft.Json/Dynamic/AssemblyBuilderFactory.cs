namespace JsonConverter.Newtonsoft.Json.Dynamic;

internal static class AssemblyBuilderFactory
{
    /// <summary>
    /// Defines a dynamic assembly that has the specified name and access rights.
    /// </summary>
    /// <param name="name">The name of the assembly.</param>
    /// <param name="access">The access rights of the assembly.</param>
    /// <returns>An object that represents the new assembly.</returns>
    public static System.Reflection.Emit.AssemblyBuilder DefineDynamicAssembly(System.Reflection.AssemblyName name, System.Reflection.Emit.AssemblyBuilderAccess access)
    {
#if (NET35 || NET40 || SILVERLIGHT)
        return AppDomain.CurrentDomain.DefineDynamicAssembly(name, access);
#else
        return System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(name, access);
#endif
    }
}