using System.Diagnostics;

namespace HowLongSince_AndrewLukashchuk;

/// <summary>
/// The NewActivityHistoryPage class represents a page for adding a new history entry for a selected activity.
/// Users can set a date, time, and description for the history entry.
/// </summary>
public partial class NewActivityHistoryPage : ContentPage
{
    NewEvent newEvent = new NewEvent();

    public int selectedEventIndex;

    /// <summary>
    /// Initializes a new instance of the NewActivityHistoryPage class for the selected activity.
    /// </summary>
    /// <param name="index">The index of the selected activity.</param>
    public NewActivityHistoryPage(int index)
	{
		InitializeComponent();
        selectedEventIndex = index;
	}

    /// <summary>
    /// Handles the "Cancel" button click event to navigate back to the previous page without saving the history entry.
    /// </summary>
    private void Button_Clicked(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }

    /// <summary>
    /// Handles the "Done" button click event to save the new history entry and update the selected activity's details.
    /// </summary>
    private void DoneButton_Clicked(object sender, EventArgs e)
    {
        newEvent.LoadEventsData();
        newEvent.LoadEventsHistoryData();

        TimeSpan defaultTime = new TimeSpan(0,0,0);
        DateTime usersDateTime;

        ActivityHistory activityHistory = new ActivityHistory();

        if (myDatePicker.Date == DateTime.Today && myTimePicker.Time == defaultTime && string.IsNullOrEmpty(descriptionEditor.Text))
        {
            DisplayAlert("Warning", "No information was provided - please provide additional information", "Ok");
        }
        else
        {
            usersDateTime = myDatePicker.Date + myTimePicker.Time;

            activityHistory.lastTimeModified = usersDateTime;
            activityHistory.description = descriptionEditor.Text;

            newEvent.activitiesHistoryList[selectedEventIndex].Insert(0, activityHistory);

            for (int i = 0; i < newEvent.activitiesList.Count; i++)
            {
                if (newEvent.activitiesList[i].Id == selectedEventIndex)
                {
                    if (newEvent.activitiesList[i].selectedDateTime <  usersDateTime)
                    {
                        newEvent.activitiesList[i].selectedDateTime = usersDateTime;
                    }
                }
            }

            newEvent.SaveEventsData();
            newEvent.SaveEventsHistoryData();

            Navigation.PopAsync();
        }
    }
}