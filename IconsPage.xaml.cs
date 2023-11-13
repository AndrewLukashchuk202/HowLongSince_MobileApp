namespace HowLongSince_AndrewLukashchuk;
/// <summary>
/// This class provides icons to choose for one activity at a time
/// </summary>
public partial class IconsPage : ContentPage
{
	public IconsPage()
	{
		InitializeComponent();
	}
    //list of string that contains a names of icons + format(png)
    List<string> listOfIcons = new List<string>() { "console.png", "friends.png", "lunch.png", "sleeping.png", "sports.png", "working.png" };
    //Property that holds one icon name at a time
    public string iconName { get; set; }
    //A flag checks if the user chose any icons otherwise, the activity gets a default one -> empty_folder.png
    public bool isIconChosen;
    /// <summary>
    /// Method that defines what icon the user chose based on its id, in this case, AutomationId is the name of icons, example: sleeping.png is AutomationId
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void icon_Clicked(object sender, EventArgs e)
    {
        ImageButton button = (ImageButton)sender;


        for (int i = 0; i < 6; i++)
        {
            if (button.AutomationId == listOfIcons[i])
            {
                iconName = listOfIcons[i];
                isIconChosen = true;
                break;
            }
        }
        Navigation.PopAsync();
    }

}