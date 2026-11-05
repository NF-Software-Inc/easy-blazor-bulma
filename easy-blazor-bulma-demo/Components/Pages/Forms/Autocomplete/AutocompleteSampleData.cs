using easy_blazor_bulma;

namespace easy_blazor_bulma_demo.Components.Pages.Forms
{
    internal class AutocompleteSampleData
    {
        /// <summary>
        /// Tooltip options that position the tooltip to the right, display an arrow, and allow multiline text.
        /// </summary>
        internal static readonly TooltipOptions TooltipDisplayMode = TooltipOptions.Right | TooltipOptions.HasArrow | TooltipOptions.Multiline;

        /// <summary>
        /// Sample applicants for the autocomplete demo.
        /// </summary>
        internal static readonly List<DemoAutocomplete> Applicants =
        [
            new DemoAutocomplete { Id = 1, Name = "Jimbo Moneybags", Age = 30, Position = "Accountant" },
            new DemoAutocomplete { Id = 2, Name = "Suzy Goldenfold", Age = 18, Position = "Accountant" },
            new DemoAutocomplete { Id = 3, Name = "Sal Mopnbucket", Age = 44, Position = "Janitor" },
            new DemoAutocomplete { Id = 4, Name = "Jill Broomhandel", Age = 61, Position = "Janitor" },
            new DemoAutocomplete { Id = 5, Name = "Reggie Cashonlee", Age = 23, Position = "Accountant" },
            new DemoAutocomplete { Id = 6, Name = "Oldie Butagoodie", Age = 72, Position = "Janitor" },
            new DemoAutocomplete { Id = 7, Name = "Rickie Notapenny", Age = 56, Position = "Accountant" },
            new DemoAutocomplete { Id = 8, Name = "Cheryl McMillions", Age = 33, Position = "Accountant" },
            new DemoAutocomplete { Id = 9, Name = "Lindsay Spicnspan", Age = 46, Position = "Janitor" },
            new DemoAutocomplete { Id = 10, Name = "Bob Skidmarks-McNastypants", Age = 55, Position = "Janitor" },
            new DemoAutocomplete { Id = 11, Name = "Pat Waxon", Age = 24, Position = "Janitor" },
            new DemoAutocomplete { Id = 12, Name = "Melissa Taxevader", Age = 36, Position = "Accountant" },
            new DemoAutocomplete { Id = 13, Name = "Larry Robberson", Age = 47, Position = "Accountant" },
            new DemoAutocomplete { Id = 14, Name = "Pat Waxoff", Age = 19, Position = "Janitor" },
            new DemoAutocomplete { Id = 15, Name = "Betty Terletson", Age = 20, Position = "Janitor" }
        ];
    }

    /// <summary>
    /// Represents a demo autocomplete item with a name, age, and position.
    /// </summary>
    /// <remarks>Equality is based on the <c>Id</c> and <c>Name</c> properties.</remarks>
    internal record class DemoAutocomplete : IAutocomplete, IEquatable<DemoAutocomplete>
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public int Age { get; init; }

        public string Position { get; init; } = string.Empty;

        /// <inheritdoc />
        public bool Equals(IAutocomplete? other)
        {
            if (other != null && Id == other.Id)
            {
                return Name == other.Name;
            }

            return false;
        }
    }

    /// <summary>
    /// Represents an autocomplete item with an identifier and name.
    /// </summary>
    internal interface IAutocomplete
    {
        public int Id { get; }
        public string Name { get; }
    }
}
