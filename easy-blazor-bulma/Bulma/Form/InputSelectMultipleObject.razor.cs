using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a select list with the provided items for selection of multiple objects.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// <see href="https://bulma.io/documentation/form/select/">Bulma Documentation</see>
/// </remarks>
public partial class InputSelectMultipleObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<List<TValue>>
{
	/// <summary>
	/// The collection of items to display in the list.
	/// </summary>
	[Parameter]
	[Required]
	public required List<TValue> Items { get; set; }

	/// <summary>
	/// A function to return the values to display in the drop-down list.
	/// </summary>
	[Parameter]
	[Required]
	public required Func<TValue, string> DisplayValue { get; set; }

	/// <summary>
	/// A function to determine an item is selected.
	/// </summary>
	[Parameter]
	public Func<ICollection<TValue>?, TValue, bool> Contains { get; set; } = (x, y) => x != null && x.Contains(y, EqualityComparer<TValue>.Default);

	/// <summary>
	/// An icon to display within the input.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; }

	/// <summary>
	/// The number of items to display in the select box.
	/// </summary>
	[Parameter]
	[Range(2, 100)]
	public int Size { get; set; } = 8;

	private readonly string[] Filter = new[] { "class" };

	private int CurrentIndex = -1;
	private int? StartIndex = null;
	private int? EndIndex = null;
	private bool CtrlMove;

	private string MainCssClass => string.Join(' ', "select is-multiple", CssClass);

	/// <inheritdoc />
	/// <exception cref="NotImplementedException"></exception>
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out List<TValue> result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
		throw new NotImplementedException();
	}

	private void OnMouseDown(int index, MouseEventArgs args)
	{
		if (args.ShiftKey == false)
			StartIndex = index;
		else
			StartIndex ??= index;
	}

	private void OnMouseUp(int index, MouseEventArgs args)
	{
		CurrentIndex = index;
		EndIndex = index;

		if (args.CtrlKey == false)
		{
			StartIndex ??= CurrentIndex;

			Value?.Clear();
			AddSelected(StartIndex.Value, EndIndex.Value);
		}
		else
		{
			StartIndex ??= CurrentIndex;

			if (Contains(Value, Items[StartIndex.Value]))
				RemoveSelected(StartIndex.Value, EndIndex.Value);
			else
				AddSelected(StartIndex.Value, EndIndex.Value);
		}
	}

	private void OnKeyUp(KeyboardEventArgs args)
	{
		if (Items.Count == 0)
			return;

		if (args.Code == "ArrowDown")
		{
			if (CurrentIndex < Items.Count - 1)
				CurrentIndex++;
		}
		else if (args.Code == "PageDown")
		{
			if (CurrentIndex + Size - 1 < Items.Count - 1)
				CurrentIndex += Size - 1;
			else
				CurrentIndex = Items.Count - 1;
		}
		else if (args.Code == "End")
		{
			CurrentIndex = Items.Count - 1;
		}
		else if (args.Code == "ArrowUp")
		{
			if (CurrentIndex > 0)
				CurrentIndex--;
			else if (CurrentIndex == -1)
				CurrentIndex = Items.Count - 1;
		}
		else if (args.Code == "PageUp")
		{
			if (CurrentIndex - Size + 1 > 0)
				CurrentIndex -= Size - 1;
			else
				CurrentIndex = 0;
		}
		else if (args.Code == "Home")
		{
			CurrentIndex = 0;
		}
		else if (args.Code != "Space")
		{
			return;
		}

		if (args.Code != "Space")
			CtrlMove = args.CtrlKey && (args.Code == "Home" || args.Code == "End" || (args.Code == "ArrowUp" && CurrentIndex != 0) || (args.Code == "ArrowDown" && CurrentIndex != Items.Count - 1));

		if (args.CtrlKey == false && args.ShiftKey == false)
		{
			StartIndex = CurrentIndex;

			if (args.Code != "Space")
			{
				Value?.Clear();
				AddSelected(CurrentIndex, CurrentIndex);
			}
			else if (CtrlMove)
			{
				ToggleSelected(CurrentIndex, CurrentIndex);
			}
		}
		else if (args.ShiftKey)
		{
			StartIndex ??= CurrentIndex;
			EndIndex = CurrentIndex;

			Value?.Clear();
			AddSelected(StartIndex.Value, EndIndex.Value);
		}
	}

	private void ToggleSelected(int start, int end)
	{
		Value ??= [];

		if (start > end)
			(end, start) = (start, end);

		for (var i = start; i <= end; i++)
		{
			if (Value.Remove(Items[i]) == false)
				Value.Add(Items[i]);
		}

		_ = ValueChanged.InvokeAsync(Value);
		EditContext?.NotifyFieldChanged(FieldIdentifier);
	}

	private void AddSelected(int start, int end)
	{
		Value ??= [];

		if (start > end)
			(end, start) = (start, end);

		for (var i = start; i <= end; i++)
		{
			if (Contains(Value, Items[i]) == false)
				Value.Add(Items[i]);
		}

		_ = ValueChanged.InvokeAsync(Value);
		EditContext?.NotifyFieldChanged(FieldIdentifier);
	}

	private void RemoveSelected(int start, int end)
	{
		Value ??= [];

		if (start > end)
			(end, start) = (start, end);

		for (var i = start; i <= end; i++)
		{
			Value.Remove(Items[i]);
		}

		_ = ValueChanged.InvokeAsync(Value);
		EditContext?.NotifyFieldChanged(FieldIdentifier);
	}
}
