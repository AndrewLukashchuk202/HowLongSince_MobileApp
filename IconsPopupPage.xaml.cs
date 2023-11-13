using CommunityToolkit.Maui.Views;
using System.Diagnostics;

namespace HowLongSince_AndrewLukashchuk;

public partial class IconsPopupPage : Popup
{
	public IconsPopupPage()
	{
		InitializeComponent();
	}

	List <string> listOfIcons = new List<string>() {"console.png", "friends.png", "lunch.png", "sleeping.png", "sports.png", "working.png"};
	public string iconName { get; set; }
	public bool isIconChosen;
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

    }


}