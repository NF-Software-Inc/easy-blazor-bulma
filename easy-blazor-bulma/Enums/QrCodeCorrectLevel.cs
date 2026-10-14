using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma;

/// <summary>
/// Error correction levels supported by qrcode.js.
/// </summary>
/// <remarks>
/// Numeric values match <c>QRCode.CorrectLevel</c> and are not ordered by recovery percentage.
/// See <see href="https://github.com/davidshimjs/qrcodejs">qrcode.js</see>.
/// </remarks>
public enum QrCodeCorrectLevel
{
	/// <summary>
	/// Recovers about 15% of damaged data. Maps to <c>QRCode.CorrectLevel.M</c>.
	/// </summary>
	[Display(Name = "Medium (~15%)")]
	M = 0,

	/// <summary>
	/// Recovers about 7% of damaged data. Maps to <c>QRCode.CorrectLevel.L</c>.
	/// </summary>
	[Display(Name = "Low (~7%)")]
	L = 1,

	/// <summary>
	/// Recovers about 30% of damaged data. Maps to <c>QRCode.CorrectLevel.H</c>.
	/// </summary>
	[Display(Name = "High (~30%)")]
	H = 2,

	/// <summary>
	/// Recovers about 25% of damaged data. Maps to <c>QRCode.CorrectLevel.Q</c>.
	/// </summary>
	[Display(Name = "Quartile (~25%)")]
	Q = 3
}
