namespace easy_blazor_bulma_demo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

	/// <inheritdoc />
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage()) { Title = "Easy Blazor Bulma" };
	}
}
