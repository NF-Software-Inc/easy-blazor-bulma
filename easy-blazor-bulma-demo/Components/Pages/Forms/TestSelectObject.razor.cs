using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma_demo.Components.Pages.Forms;

public partial class TestSelectObject : ComponentBase
{
	private readonly PageModel InputModel = new()
	{
		SelectedItem1 = AllItems[2]
	};

	private readonly TooltipOptions TooltipDisplayMode = TooltipOptions.Right | TooltipOptions.HasArrow | TooltipOptions.Multiline;

	private static readonly List<DemoObject> AllItems =
	[
		new DemoObject { Id = 1, Name = "Jimbo Moneybags", Age = 30, Position = "Accountant" },
		new DemoObject { Id = 2, Name = "Suzy Goldenfold", Age = 18, Position = "Accountant" },
		new DemoObject { Id = 3, Name = "Sal Mopnbucket", Age = 44, Position = "Janitor" },
		new DemoObject { Id = 4, Name = "Jill Broomhandel", Age = 61, Position = "Janitor" },
		new DemoObject { Id = 5, Name = "Reggie Cashonlee", Age = 23, Position = "Accountant" },
		new DemoObject { Id = 6, Name = "Oldie Butagoodie", Age = 72, Position = "Janitor" },
		new DemoObject { Id = 7, Name = "Rickie Notapenny", Age = 56, Position = "Accountant" },
		new DemoObject { Id = 8, Name = "Cheryl McMillions", Age = 33, Position = "Accountant" },
		new DemoObject { Id = 9, Name = "Lindsay Spicnspan", Age = 46, Position = "Janitor" },
		new DemoObject { Id = 10, Name = "Bob Skidmarks-McNastypants", Age = 55, Position = "Janitor" },
		new DemoObject { Id = 11, Name = "Pat Waxon", Age = 24, Position = "Janitor" },
		new DemoObject { Id = 12, Name = "Melissa Taxevader", Age = 36, Position = "Accountant" },
		new DemoObject { Id = 13, Name = "Larry Robberson", Age = 47, Position = "Accountant" },
		new DemoObject { Id = 14, Name = "Pat Waxoff", Age = 19, Position = "Janitor" },
		new DemoObject { Id = 15, Name = "Betty Terletson", Age = 20, Position = "Janitor" }
	];

	private IEnumerable<DemoObject> GetObjects()
	{
		return AllItems.Where(x => x.Position == "Accountant");
	}

	private class PageModel
	{
		[Display(Name = "Janitor Applicant", Description = "Please select the name of the applicant to hire for the Janitor position.")]
		public required DemoObject SelectedItem1 { get; set; }

		[Display(Name = "Accountant Applicant", Description = "Please select the name of the applicant to hire for the Accountant position.")]
		public DemoObject? SelectedItem2 { get; set; }

		[Display(Name = "Enum Test", Description = "Input to test modifying a non-nullable enum value.")]
		public TestEnum EnumSelectTest { get; set; } = TestEnum.One;

		[Display(Name = "Rejected Applicant(s)", Description = "Please select the name(s) of the applicant to send a rejection letter to.")]
		public List<DemoObject> SelectedItems { get; set; } = [AllItems[3], AllItems[7]];
	}

	private class DemoObject
	{
		public int Id { get; init; }

		public required string Name { get; init; }

		public int Age { get; init; }

		public required string Position { get; init; }
	}

	private enum TestEnum
	{
		[Display(Name = "Zero")]
		None,
		One,
		Two,
		Three,
		Four,
		Five,
		Six,
		Seven,
		Eight
	}
}
