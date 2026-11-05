namespace easy_blazor_bulma_demo.Services
{
    public static class ExampleSource
    {
        /// <summary>
        /// Retrieves the contents of an embedded source file.
        /// </summary>
        /// <param name="fileName">The name of the embedded source file.</param>
        /// <returns>The trimmed file contents, or a message indicating that the source was not found.</returns>
        public static string Get(string fileName)
        {
            using var stream = typeof(ExampleSource).Assembly.GetManifestResourceStream("Examples." + fileName);

            if (stream == null)
                return $"Source not found: {fileName}";

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd().Trim();
        }

        /// <summary>
        /// Gets the source code of an example component, e.g. Get&lt;AutocompleteBasicExample&gt;().
        /// </summary>
        /// <typeparam name="TExample">The type of the example component.</typeparam>
        /// <returns>The source code of the example component.</returns>
        public static string Get<TExample>() => Get(typeof(TExample));

        /// <summary>
        /// Gets the source code of an example component by its type.
        /// </summary>
        /// <param name="exampleType">The type of the example component.</param>
        /// <returns>The source code of the example component.</returns>
        public static string Get(Type exampleType) => Get(exampleType.Name + ".razor");
    }
}
