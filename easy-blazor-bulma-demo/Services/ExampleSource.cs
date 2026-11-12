namespace easy_blazor_bulma_demo.Services
{
    /// <summary>
    /// Reads the raw source code of Blazor example components embedded in this assembly.
    /// </summary>
    /// <remarks>
    /// Works alongside the <c>&lt;EmbeddedResource&gt;</c> rule in the <c>.csproj</c> file, which bundles
    /// all <c>.razor</c> files under <c>Components\Pages</c> using the naming pattern <c>Examples.&lt;FileName&gt;.razor</c>.
    /// </remarks>
    public static class ExampleSource
    {
        /// <summary>
        /// Reads the raw source code of an embedded <c>.razor</c> file by its file name.
        /// </summary>
        /// <param name="fileName">The file name including extension (e.g., <c>"AutocompleteBasicExample.razor"</c>).</param>
        /// <returns>The trimmed file contents, or a fallback message if no matching resource was embedded.</returns>
        public static string Get(string fileName)
        {
            // In .csproj file, LogicalName="Examples.%(Filename)%(Extension)" flattens all embedded .razor files under the "Examples." prefix, regardless of their original subfolder.
            using var stream = typeof(ExampleSource).Assembly.GetManifestResourceStream("Examples." + fileName);

            if (stream == null)
                return $"Source not found: {fileName}";

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd().Trim();
        }

        /// <summary>
        /// Reads the raw source code of an example component using its component type (e.g., <c>ExampleSource.Get&lt;AutocompleteBasicExample&gt;()</c>).
        /// </summary>
        /// <typeparam name="TExample">The Blazor component type whose <c>.razor</c> file should be read.</typeparam>
        /// <returns>The trimmed source code of the example component.</returns>
        public static string Get<TExample>() => Get(typeof(TExample));

        /// <summary>
        /// Reads the raw source code of an example component by matching its class name to a <c>.razor</c> file.
        /// </summary>
        /// <remarks>
        /// Assumes the component's class name matches its file name (e.g., <c>ButtonDemo</c> maps to <c>ButtonDemo.razor</c>).
        /// </remarks>
        /// <param name="exampleType">The <see cref="Type"/> of the example component.</param>
        /// <returns>The trimmed source code of the example component.</returns>
        public static string Get(Type exampleType) => Get(exampleType.Name + ".razor");
    }
}
