namespace easy_blazor_bulma;

/// <summary>
/// A list of options to configure an <see cref="InputAutocomplete{TValue}"/>.
/// </summary>
[Flags]
public enum InputAutocompleteOptions
{
    /// <summary>
	/// Creates an input with no options set.
	/// </summary>
	None = 0b_00000000_00000000_00000000_00000000,

	/// <summary>
	/// A popout will be displayed when hovering over the component.
	/// </summary>
	HoverPopout = 0b_00000000_00000000_00000000_00000001,

	/// <summary>
	/// A popout will be displayed when clicking the component or focus enters the component.
	/// </summary>
	ClickPopout = 0b_00000000_00000000_00000000_00000010,

	/// <summary>
	/// A popout will be displayed after typing in the component.
	/// </summary>
	TypePopout = 0b_00000000_00000000_00000000_00000100,

	/// <summary>
	/// Sets the popout to display above the input.
	/// </summary>
	PopoutTop = 0b_00000000_00000000_00000000_00001000,

    /// <summary>
    /// Sets the popout to display below the input.
    /// </summary>
    PopoutBottom = 0b_00000000_00000000_00000000_00010000,

    /// <summary>
    /// Sets the popout to align with the left of the input.
    /// </summary>
    PopoutLeft = 0b_00000000_00000000_00000000_00100000,

    /// <summary>
    /// Sets the popout to align with the right the input.
    /// </summary>
    PopoutRight = 0b_00000000_00000000_00000000_01000000,

    /// <summary>
	/// Automatically sets a success or failure border on the input based on user input.
	/// </summary>
	UseAutomaticStatusColors = 0b_00000000_00000000_00000000_10000000,

    /// <summary>
    /// Automatically selects a match when exiting the component.
    /// </summary>
    AutoSelectOnExit = 0b_00000000_00000000_00000001_00000000,

    /// <summary>
    /// Automatically selects a match when typing in the component.
    /// </summary>
    AutoSelectOnInput = 0b_00000000_00000000_00000010_00000000,

	/// <summary>
	/// Automatically selects the currently highlighted item when exiting the component.
	/// </summary>
	AutoSelectCurrent = 0b_00000000_00000000_00000100_00000000,

    /// <summary>
    /// Automatically selects an item if there is an exact match on the display text when exiting the component.
    /// </summary>
    AutoSelectExact = 0b_00000000_00000000_00001000_00000000,

    /// <summary>
    /// Automatically selects the item whose display text is closest to the input when exiting the component.
    /// </summary>
    AutoSelectClosest = 0b_00000000_00000000_00010000_00000000,

	/// <summary>
	/// Allows selecting the same item more than once in <see cref="InputAutocompleteMultiple{TValue}"/>.
	/// </summary>
	AllowSameItem = 0b_00000000_00000000_00100000_00000000
}
