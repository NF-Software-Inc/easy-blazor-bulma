using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma_demo.Components.Pages.Forms;

/// <summary>
/// Demonstrates dictionary and list-based radio group configurations with independent selections.
/// </summary>
public partial class TestRadioGroupObject : ComponentBase
{
	private readonly Dictionary<string, DemoObject?> EmployeeDictionaryOptions = new()
	{
		["Nobody"] = null,
		["Jimbo Moneybags"] = new DemoObject { Id = 1, Name = "Jimbo Moneybags", Age = 30, Position = "Accountant" },
		["Suzy Goldenfold"] = new DemoObject { Id = 2, Name = "Suzy Goldenfold", Age = 18, Position = "Accountant" }
	};

	private readonly List<DemoObject?> EmployeeListOptions =
	[
		null,
		new DemoObject { Id = 1, Name = "Jimbo Moneybags", Age = 30, Position = "Accountant" },
		new DemoObject { Id = 2, Name = "Suzy Goldenfold", Age = 18, Position = "Accountant" }
	];

	private readonly PlaceholderModel InputModel = new();

	private void OnButtonClicked()
	{
		InputModel.Selected1 = EmployeeDictionaryOptions["Jimbo Moneybags"];
	}

	private class DemoObject
	{
		public int Id { get; init; }

		public required string Name { get; init; }

		public int Age { get; init; }

		public required string Position { get; init; }
	}

	private class PlaceholderModel
	{
		public DemoObject? Selected1 { get; set; }

		public DemoObject? Selected2 { get; set; }
	}
}
