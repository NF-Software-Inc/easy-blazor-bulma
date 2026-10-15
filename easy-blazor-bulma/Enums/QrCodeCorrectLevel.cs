using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma;

/// <summary>
/// Error correction levels supported QR code generation.
/// </summary>
/// <remarks>
/// Higher error correction levels (Q and H) reserve more of the QR code's grid for backup redundancy codewords, which reduces the amount of primary data you can store or forces the code to use a larger grid version.
/// </remarks>
public enum QrCodeCorrectLevel
{
	/// <summary>
	/// Ideal for standard business cards, flyers, indoor signage, brochures, and general commercial
	/// </summary>
	[Display(Name = "Medium (~15%)")]
	M = 0,

	/// <summary>
	/// Best for clean, digital-only environments with massive data payloads where maximum storage capacity is needed.
	/// </summary>
	[Display(Name = "Low (~7%)")]
	L = 1,

	/// <summary>
	/// Best for harsh industrial environments, warehouses, greasy factory floors, or when placing a custom brand logo in the center of the QR code.
	/// </summary>
	[Display(Name = "High (~30%)")]
	H = 2,

	/// <summary>
	/// Best for outdoor displays, product packaging, or surfaces exposed to moderate physical wear and handling.
	/// </summary>
	[Display(Name = "Quartile (~25%)")]
	Q = 3
}
