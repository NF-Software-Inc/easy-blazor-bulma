using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a select list with the provided items.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// <see href="https://bulma.io/documentation/form/select/">Bulma Documentation</see>
/// </remarks>
public partial class InputSelectObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TItem, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>
{
	/// <summary>
	/// The collection of items to display in the list.
	/// </summary>
	[Parameter]
	[EditorRequired]
	public required IEnumerable<TItem> Items { get; set; }
	private IReadOnlyList<TItem> ItemsList = [];

	/// <summary>
	/// A function to return the values to display in the drop-down list.
	/// </summary>
	[Parameter]
	[EditorRequired]
	public required Func<TItem, string> DisplayValue { get; set; }

	/// <summary>
	/// A function to return the bound value for items provided in the <see cref="Items"/> collection.
	/// </summary>
	/// <remarks>
	/// When omitted, <typeparamref name="TItem"/> and <typeparamref name="TValue"/> must be identical.
	/// </remarks>
	[Parameter]
	public Func<TItem, TValue>? ValueSelector { get; set; }

	/// <summary>
	/// A function to determine whether two items are equal.
	/// </summary>
	[Parameter]
	public Func<TValue, TValue?, bool> AreEqual { get; set; } = EqualityComparer<TValue>.Default.Equals;

	/// <summary>
	/// An icon to display within the input.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; } = "list";

	/// <summary>
	/// Specifies the text to display for the null option.
	/// </summary>
	[Parameter]
	public string NullText { get; set; } = "Null";

	/// <summary>
	/// Applies styles to the input.
	/// </summary>
	[Parameter]
	public InputStatus DisplayStatus { get; set; }

	/// <inheritdoc cref="InputDateTimeOptions.UseAutomaticStatusColors"/>
	[Parameter]
	public bool UseAutomaticStatusColors { get; set; } = true;

	/// <summary>
	/// Gets or sets the associated <see cref="ElementReference"/>.
	/// <para>
	/// May be <see langword="null"/> if accessed before the component is rendered.
	/// </para>
	/// </summary>
	[DisallowNull]
	public ElementReference? Element { get; private set; }

	private readonly string[] Filter = ["class"];

	private static readonly bool UsesLegacyIdentitySelection = typeof(TItem) == typeof(TValue);
	private readonly Type UnderlyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
	private bool IsNullable;

	private string MainCssClass
	{
		get
		{
			var css = "select";

			if (DisplayStatus.HasFlag(InputStatus.BackgroundDanger))
				css += " is-danger";
			else if (DisplayStatus.HasFlag(InputStatus.BackgroundWarning))
				css += " is-warning";
			else if (DisplayStatus.HasFlag(InputStatus.BackgroundSuccess))
				css += " is-success";

			return string.Join(' ', css, CssClass);
		}
	}

	/// <inheritdoc/>
	protected override void OnInitialized()
	{
		if (UnderlyingType.GetTypeInfo().IsValueType)
			IsNullable = Nullable.GetUnderlyingType(typeof(TValue)) != null;
		else if (FieldIdentifier.Model != null && string.IsNullOrEmpty(FieldIdentifier.FieldName) == false)
			IsNullable = IsMemberNullable(FieldIdentifier.Model.GetType(), FieldIdentifier.FieldName);
	}

	private static bool IsMemberNullable(Type modelType, string memberName)
	{
		var context = new NullabilityInfoContext();
		var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

		for (var type = modelType; type != null && type != typeof(object); type = type.BaseType)
		{
			var member = (MemberInfo?)type.GetProperty(memberName, flags) ?? (MemberInfo?)type.GetField(memberName, flags);

			if (member == null)
				continue;

			var info = member switch
			{
				PropertyInfo property => context.Create(property),
				FieldInfo field => context.Create(field),
				_ => null
			};

			if (info == null)
				continue;
			else if (info.WriteState != NullabilityState.Unknown)
				return info.WriteState == NullabilityState.Nullable;
			else if (info.ReadState != NullabilityState.Unknown)
				return info.ReadState == NullabilityState.Nullable;
		}

		return false;
	}

	/// <inheritdoc/>
	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		if (ValueSelector == null && UsesLegacyIdentitySelection == false)
			throw new InvalidOperationException($"{nameof(ValueSelector)} is required when {nameof(TItem)} and {nameof(TValue)} are different types.");

		ItemsList = Items as IReadOnlyList<TItem> ?? Items.ToList();
	}

	/// <inheritdoc/>
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
		if (UseAutomaticStatusColors)
			ResetStatus();

		if (IsNullable && string.IsNullOrEmpty(value))
		{
			if (UseAutomaticStatusColors)
				DisplayStatus |= InputStatus.BackgroundSuccess;

			result = default!;
			validationErrorMessage = null;
			return true;
		}

		if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var i) && i >= 0 && i < ItemsList.Count)
		{
			if (UseAutomaticStatusColors)
				DisplayStatus |= InputStatus.BackgroundSuccess;

			result = GetValue(ItemsList[i]);
			validationErrorMessage = null;
			return true;
		}

		if (UseAutomaticStatusColors)
			DisplayStatus |= InputStatus.BackgroundDanger;

		result = default;
		validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "No match could be found in the {0} field.", DisplayName ?? FieldIdentifier.FieldName);
		return false;
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(TValue? value)
	{
		if (value == null)
			return string.Empty;

		for (var i = 0; i < ItemsList.Count; i++)
			if (AreEqual(GetValue(ItemsList[i]), value))
				return i.ToString(CultureInfo.InvariantCulture);

		return string.Empty;
	}

	private TValue GetValue(TItem item)
	{
		if (ValueSelector != null)
			return ValueSelector(item);

		if (UsesLegacyIdentitySelection)
			return (TValue)(object?)item!;

		throw new InvalidOperationException($"{nameof(ValueSelector)} is required when {nameof(TItem)} and {nameof(TValue)} are different types.");
	}

	private void ResetStatus()
	{
		DisplayStatus &= ~InputStatus.BackgroundDanger;
		DisplayStatus &= ~InputStatus.BackgroundWarning;
		DisplayStatus &= ~InputStatus.BackgroundSuccess;
	}
}
