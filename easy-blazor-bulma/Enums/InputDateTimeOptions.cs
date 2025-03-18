namespace easy_blazor_bulma;

/// <summary>
/// A list of options to configure an <see cref="InputDateTime{TValue}"/>.
/// </summary>
[Flags]
public enum InputDateTimeOptions
{
	/// <summary>
	/// Creates an input with no options set.
	/// </summary>
	None = 0b_00000000_00000000_00000000_00000000,

	/// <summary>
	/// No popout will be displayed for the component.
	/// </summary>
	NoPopout = 0b_00000000_00000000_00000000_00000001,

	/// <summary>
	/// A popout will be displayed when hovering over the component.
	/// </summary>
	HoverPopout = 0b_00000000_00000000_00000000_00000010,

	/// <summary>
	/// A popout will be displayed when clicking the component or focus enters the component.
	/// </summary>
	ClickPopout = 0b_00000000_00000000_00000000_00000100,

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
	/// Updates the bound value when the popout values are changed, otherwise requires clicking Accept button.
	/// </summary>
	UpdateOnPopoutChange = 0b_00000000_00000000_00000000_10000000,

	/// <summary>
	/// Automatically sets a success or failure border on the input based on user input.
	/// </summary>
	UseAutomaticStatusColors = 0b_00000000_00000000_00000001_00000000,

	/// <summary>
	/// Specifies whether the popout will display the Accept button. Only required when <see cref="UpdateOnPopoutChange"/> is not active.
	/// </summary>
	ShowAcceptButton = 0b_00000000_00000000_00000010_00000000,

	/// <summary>
	/// Specifies whether the popout will display the Now or Today button.
	/// </summary>
	ShowNowButton = 0b_00000000_00000000_00000100_00000000,

	/// <summary>
	/// Specifies whether the popout will display the Reset button.
	/// </summary>
	ShowResetButton = 0b_00000000_00000000_00001000_00000000,

	/// <summary>
	/// Specifies whether the popout will display the Cancel button.
	/// </summary>
	ShowCancelButton = 0b_00000000_00000000_00010000_00000000,

	/// <summary>
	/// Specifies whether the popout will display the date picker section.
	/// </summary>
	ShowDate = 0b_00000000_00000000_00100000_00000000,

	/// <summary>
	/// Specifies whether the popout will display the hours column.
	/// </summary>
	ShowHours = 0b_00000000_00000000_01000000_00000000,

	/// <summary>
	/// Specifies whether the popout will display the minutes column.
	/// </summary>
	ShowMinutes = 0b_00000000_00000000_10000000_00000000,

	/// <summary>
	/// Specifies whether the popout will display the seconds column.
	/// </summary>
	ShowSeconds = 0b_00000000_00000001_00000000_00000000,

	/// <summary>
	/// Closes the popout when a date value is clicked.
	/// </summary>
	CloseOnDateClicked = 0b_00000000_00000010_00000000_00000000,

	/// <summary>
	/// Checks for common input errors when the text field is used.
	/// </summary>
    ValidateTextInput = 0b_00000000_00000100_00000000_00000000,

	/// <summary>
	/// Parses decimal values as minutes or seconds. The portion after the decimal will become a percentage of 60 minutes or 60 seconds based on the input string.
	/// </summary>
    ConvertDecimals = 0b_00000000_00001000_00000000_00000000,

	/// <summary>
	/// Indicates whether to display the full date and time of the selected value below the input.
	/// </summary>
	ShowFullValueBelow = 0b_00000000_00010000_00000000_00000000
}
