namespace HowLongSince_AndrewLukashchuk;

// <summary>
/// Represents the Settings Page where the user can configure various app settings.
/// </summary>
public partial class SettingsPage : ContentPage
{
    // Properties to store user settings
    public string userNickname;
    public bool darkMode;

    //colors - blue, orange, darkgreen
    public List<string> colors = new List<string>() { "#85C1E9", "#D4AC0D", "#1D8348"};
    public int colorIndex = 0;

    public List<int> fonts = new List<int>() {17, 21, 25};
    public int fontIndex = 0;

    // List of buttons used for settings
    public List<Button> settingsButtons;

	public SettingsPage()
	{
		InitializeComponent();
        settingsButtons = new List<Button>() {backButton, personalInformationButton, appThemeButton, appColorButton, fontButton };
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadSettingsPreferences();
    }

    private void backButton_Clicked(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }

    /// <summary>
    /// Handles the click event for the personal information button and allows the user to set their username.
    /// </summary>
    private async void personalInformationButton_Clicked(object sender, EventArgs e)
    {
        userNickname = await DisplayPromptAsync("Personal Information", "Provide your username please");
        userLabel.Text = userNickname;
        Preferences.Default.Set("settingsName", userNickname);       
    }

    /// <summary>
    /// Loads user settings preferences from application storage and updates the UI accordingly.
    /// </summary>
    public void LoadSettingsPreferences()
    {
        userNickname = Preferences.Default.Get("settingsName", userNickname);
        darkMode = Preferences.Default.Get("appThemeMode", darkMode);
        colorIndex = Preferences.Default.Get("colorIndex", colorIndex);
        fontIndex = Preferences.Default.Get("fontIndex", fontIndex);

        userLabel.Text = userNickname;

        if (darkMode)
        {
            App.Current.UserAppTheme = AppTheme.Dark;

        }
        else
        {
            App.Current.UserAppTheme = AppTheme.Light;
        }

        for (int i = 0; i < settingsButtons.Count; i++)
        {
            settingsButtons[i].TextColor = Color.FromArgb(colors[colorIndex % colors.Count]);
        }
        userLabel.TextColor = Color.FromArgb(colors[colorIndex % colors.Count]);

        for (int i = 0; i < settingsButtons.Count; i++)
        {
            settingsButtons[i].FontSize = fonts[fontIndex % colors.Count];
        }
        userLabel.FontSize = fonts[fontIndex % colors.Count];

    }

    /// <summary>
    /// Handles the click event for the app theme button and toggles between light and dark modes.
    /// </summary>
    private void appThemeButton_Clicked(object sender, EventArgs e)
    {
        darkMode = !darkMode;
        if (darkMode)
        {
            App.Current.UserAppTheme = AppTheme.Dark;
            Preferences.Default.Set("appThemeMode", darkMode);
        }
        else
        {
            App.Current.UserAppTheme = AppTheme.Light;
            Preferences.Default.Set("appThemeMode", darkMode);
        }
    }

    /// <summary>
    /// Handles the click event for the app color button and cycles through color options.
    /// </summary>
    private void appColorButton_Clicked(object sender, EventArgs e)
    {
        colorIndex++;

        for (int i = 0; i < settingsButtons.Count; i++)
        {
            settingsButtons[i].TextColor = Color.FromArgb(colors[colorIndex % colors.Count]);
        }
        userLabel.TextColor = Color.FromArgb(colors[colorIndex % colors.Count]);

        Preferences.Default.Set("colorIndex", colorIndex);
    }

    /// <summary>
    /// Handles the click event for the font button and cycles through font size options.
    /// </summary>
    private void fontButton_Clicked(object sender, EventArgs e)
    {
        fontIndex++;

        for (int i = 0; i < settingsButtons.Count; i++)
        {
            settingsButtons[i].FontSize = fonts[fontIndex % fonts.Count];
        }
        userLabel.FontSize = fonts[fontIndex % colors.Count];

        Preferences.Default.Set("fontIndex", fontIndex);
    }
}