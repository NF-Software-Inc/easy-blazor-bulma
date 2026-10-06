using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma_demo.Components.Pages.Forms;

/// <summary>
/// Demonstrates dictionary and list-based radio groups with independent selections and dynamic disabled or read-only attributes.
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

	private Dictionary<string, object> ControlAttributes = [];

	private string ControlState
	{
		get
		{
			if (ControlAttributes.ContainsKey("disabled"))
				return "Disabled";
			else if (ControlAttributes.ContainsKey("readonly"))
				return "Read-Only";
			else
				return "Enabled";
		}
	}

	private void EnableControl()
	{
		ControlAttributes = [];
	}

	private void DisableControl()
	{
		ControlAttributes = new() { ["disabled"] = "disabled" };
	}

	private void MakeControlReadOnly()
	{
		ControlAttributes = new() { ["readonly"] = "readonly" };
	}

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

		public DemoObject? Selected3 { get; set; }
	}
}
