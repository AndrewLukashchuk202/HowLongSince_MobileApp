using CommunityToolkit.Maui.Core.Extensions;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace HowLongSince_AndrewLukashchuk;

/// <summary>
/// The ActivityPage class represents a page for managing and viewing details of a specific user activity. 
/// Users can view the activity's history, add new history entries, reset history, and delete the activity.
/// </summary>
public partial class ActivityPage : ContentPage
{
    NewEvent newEvent = new NewEvent();

    public int selectedActivityByIndex;

    public DateTime currentSystemTime = DateTime.Now;

    /// <summary>
    /// Initializes a new instance of the ActivityPage class for the selected activity.
    /// </summary>
    /// <param name="selectedIndexOfActivity">The unique identifier of the selected activity.</param>
    public ActivityPage(int selectedIndexOfActivity)
	{
		InitializeComponent();
        selectedActivityByIndex = selectedIndexOfActivity;
    }

    /// <summary>
    /// Handles the "Back" button click event to navigate back to the previous page.
    /// </summary>
    private void BackButton_Clicked(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }

    /// <summary>
    /// Overrides the behavior when the page appears. It sorts and displays the activity history and counts the history entries.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        SortHistoryList();
        DisplayActivities();
        CountHistoryActivities();
    }

    /// <summary>
    /// Displays the selected activity's details and its history entries.
    /// </summary>
    public void DisplayActivities()
    {
        newEvent.LoadEventsData();
        newEvent.LoadEventsHistoryData();
        Debug.WriteLine(selectedActivityByIndex);
        //in case a user deletes one object we need to check the id of the rest objects
        foreach (Activity activity in newEvent.activitiesList) 
        {
            if (activity.Id == selectedActivityByIndex)
            {
                iconImage.Source = activity.iconName;
                titleLabel.Text = activity.title;
                timeDifferenceLabel.Text = activity.timeDifference;

                activitiesListView.ItemsSource = newEvent.activitiesHistoryList[selectedActivityByIndex];
            }
        }
    }

    /// <summary>
    /// Counts and displays the number of history activities associated with the selected user activity.
    /// </summary>
    public void CountHistoryActivities()
    {
        int count = 0;
        foreach (ActivityHistory activityHistory in activitiesListView.ItemsSource)
        {
            count++;
        }
        historyCountLabel.Text = count.ToString();
    }

    /// <summary>
    /// Handles the "Done" button click event. It updates the activity's history, timestamps, and counts.
    /// </summary>
    private void doneButton_Clicked(object sender, EventArgs e)
    {
        ActivityHistory activityHistory = new ActivityHistory();

        activityHistory.lastTimeModified = DateTime.Now;

        for (int i = 0; i < newEvent.activitiesList.Count; i++)
        {
            if (newEvent.activitiesList[i].Id == selectedActivityByIndex)
            {
                newEvent.activitiesList[i].selectedDateTime = DateTime.Now;
                break;
            }
        }

        newEvent.activitiesHistoryList[selectedActivityByIndex].Add(activityHistory);
        newEvent.SaveEventsHistoryData();
        newEvent.SaveEventsData();

        CountHistoryActivities();
        SortHistoryList();
        DisplayActivities();
    }

    /// <summary>
    /// Handles the "Reset" button click event. It clears the activity's history and updates the timestamp.
    /// </summary>
    private void resetButton_Clicked(object sender, EventArgs e)
    {
        newEvent.activitiesHistoryList[selectedActivityByIndex].Clear();
        newEvent.SaveEventsHistoryData();
        
        //updating difference time to - none, since we delete all history
        for (int i = 0; i < newEvent.activitiesList.Count; i++)
        {
            if (newEvent.activitiesList[i].Id == selectedActivityByIndex)
            {
                newEvent.activitiesList[i].selectedDateTime = DateTime.MinValue;
                newEvent.SaveEventsData();
                break;
            }
        }

        CountHistoryActivities();
        DisplayActivities();

    }

    /// <summary>
    /// Handles the "Setting" button click event. It allows users to delete the entire activity feed.
    /// </summary>
    private async void settingButton_Clicked(object sender, EventArgs e)
    {
        bool result = await DisplayAlert("Warning!", "Do you want to delete acitivity feed?", "Yes", "Cancel");
        if (result)
        {
            newEvent.activitiesHistoryList[selectedActivityByIndex].Clear();
            for (int i = 0; i < newEvent.activitiesList.Count; i++) 
            {
                if (newEvent.activitiesList[i].Id == selectedActivityByIndex)
                {
                    newEvent.activitiesList.RemoveAt(i);
                }
            }

            newEvent.SaveEventsData();
            newEvent.SaveEventsHistoryData();
            await Navigation.PopAsync();
        }
    }

    /// <summary>
    /// Handles the tap event on an item in the activity's history list. It displays the description of the history entry if available.
    /// </summary>
    private void activitiesListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        newEvent.LoadEventsData();
        newEvent.LoadEventsHistoryData();
        if (e.Item is  ActivityHistory selectedHistory) 
        {
            if (!string.IsNullOrEmpty(selectedHistory.description))
            {
                DisplayAlert("Description", selectedHistory.description, "Ok");
            }
        }
    }

    /// <summary>
    /// Sorts the activity's history list by the last modified timestamp in descending order.
    /// </summary>
    public void SortHistoryList()
    {
        newEvent.LoadEventsHistoryData();

        List<ObservableCollection<ActivityHistory>> updatedLists = new List<ObservableCollection<ActivityHistory>>();

        foreach (var innerList in newEvent.activitiesHistoryList)
        {
            var sortedList = new ObservableCollection<ActivityHistory>(innerList.OrderByDescending(item => item.lastTimeModified));
            updatedLists.Add(sortedList);
        }

        newEvent.activitiesHistoryList = new ObservableCollection<ObservableCollection<ActivityHistory>>(updatedLists);

        newEvent.SaveEventsHistoryData();
    }

    /// <summary>
    /// Handles the "New" button click event to add a new history entry for the selected activity.
    /// </summary>
    private void newButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new NewActivityHistoryPage(selectedActivityByIndex));
    }
}