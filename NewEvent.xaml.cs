using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace HowLongSince_AndrewLukashchuk;

/// <summary>
/// Class (NewEvent) creates an activity directly, where the user sets personal parameters for this activity, icon, date, time, description.
/// To do this, we use a list of data type <Activity>, where takes a new class (Activity) and save the list to keep track of all the created activities as well as history of chosen activity using <ActivityHistory> class
/// </summary>
public partial class NewEvent : ContentPage
{
    public SettingsPage settingsPage = new SettingsPage();

    public List<Label> labels;

    public IconsPage iconsPage = new IconsPage();
    public DateTime currentSystemDateTime = DateTime.Now;

 
    public ObservableCollection<Activity> activitiesList = new ObservableCollection<Activity>();
    public ObservableCollection<ObservableCollection<ActivityHistory>> activitiesHistoryList = new ObservableCollection<ObservableCollection<ActivityHistory>>();

    //Creating 2 different file for keeping tracks of activities and histories attached to each activity
    public static String fileName = "events.txt";
    public static String fileName2 = "events_history.txt";
    public static String jsonString;
    public static String jsonString2;

    //In the future a user may delete some of activities, for that purpose activitiesIdCount keeps track of all existing activities to create a new unique id for a new activity
    public int activitiesIdCount = 0;
    public NewEvent()
    {
        InitializeComponent();
        BindingContext = this;
        labels = new List<Label>() {explanationLabel, titleLabel, dateTimeLabel, descriptionLabel };
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    /// <summary>
    /// A method opens new page (iconPage) of IconsPage class
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ChooseIconButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(iconsPage);
    }

    protected override void OnAppearing()
    {
        //getting an icon 
        base.OnAppearing();
        UpdateIcon();
        //loading preferences -> number of activites using (activitiesIdCount) variable
        LoadIdPreferences();
        //loading json file
        LoadEventsData();
        LoadEventsHistoryData();
        //updating design
        UpdateNewEventPageDesign();
    }
    /// <summary>
    /// A method that updates icon based on what user chose for his activity
    /// </summary>
    public void UpdateIcon()
    {
        if (iconsPage.isIconChosen)
        {
            chosenIcon.Source = iconsPage.iconName;
        }
    }

    /// <summary>
    /// A method that after pressing Done button starts proccessing incoming data. Creates Activity and ActivityHistory objects to make a new activity
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DoneButton_Clicked_1(object sender, EventArgs e)
    {
        Activity activity = new Activity();
        ActivityHistory activityHistory = new ActivityHistory();

        DateTime usersDateTime;

        activity.iconName = iconsPage.iconName;

        
        usersDateTime = myDatePicker.Date + myTimePicker.Time;

        //default time to see if user's time is different from defaultPickerTime variable
        TimeSpan defaultPickerTime = new TimeSpan(0, 0, 0);

        //If user provided a new time or a new date then we assign new value to Activity objects properties otherwise we assign current date and time
        if (usersDateTime.Day != currentSystemDateTime.Day || (usersDateTime.Day == currentSystemDateTime.Day && usersDateTime.TimeOfDay != defaultPickerTime))
        {
            activity.selectedDateTime = usersDateTime;
            activity.timeDifference = CalculateTimeDifference(usersDateTime);

            activityHistory.lastTimeModified = usersDateTime;
            
        }
        else
        {
            activity.selectedDateTime = currentSystemDateTime;
            activityHistory.lastTimeModified = currentSystemDateTime;
        }

        //if no icon is chosen, default one will be assigned to iconName property 
        if (!iconsPage.isIconChosen)
        {
            activity.iconName = "empty_folder.png";
        }

        //checking for description input
        if (!string.IsNullOrEmpty(descriptionEditor.Text))
        {
            activityHistory.description = descriptionEditor.Text;
        }

        //checking if title was given otherwise we prompt user to provide a title| If yes -> we assign title and id to Activity object propersties, add it to observable lists to hold information about existing activities -> save data of both lists converting it to
        //txt files using json format -> saving preferences (number of created activities)
        if (string.IsNullOrEmpty(titleEditor.Text))
        {
            ShowWarningMessage("Warning!", "No title was provided - please re-enter you title");
            titleEditor.Focus();
        }
        else
        {
            activity.title = titleEditor.Text;
            activity.Id = activitiesIdCount;
            activitiesList.Add(activity);
            activitiesHistoryList.Add(new ObservableCollection<ActivityHistory> { activityHistory });

            SaveEventsData();
            SaveEventsHistoryData();

            titleEditor.Text = string.Empty;
            activitiesIdCount++;

            Preferences.Default.Set("activitiesIdCount", activitiesIdCount);

            Navigation.PopAsync();
        }
    }

    /// <summary>
    ///A method that calculates how much time has passed since the latest update of a chosen activity using the user's date and time and comparing it to the current system time. In case the user reset history, 
    ///the new value will be none as the selectedDateTime property of (Activity) the class will be empty
    /// </summary>
    /// <param name="usersDateTime"></param>
    /// <returns></returns>
    public string CalculateTimeDifference(DateTime usersDateTime)
    {
        string result = "none";

        if (usersDateTime != DateTime.MinValue)
        {
            TimeSpan timeDifference = currentSystemDateTime - usersDateTime;

            if (timeDifference.TotalDays >= 365) //more than a year
            {
                int years = (int)(timeDifference.TotalDays / 365);
                int months = (int)(timeDifference.TotalDays % 365 / 30);

                result = $"{years}y {months}m ago";
            }
            else if (timeDifference.TotalDays >= 30) // more than a month
            {
                int months = (int)(timeDifference.TotalDays / 30);
                int days = (int)(timeDifference.TotalDays % 30);

                result = $"{months}m {days}d ago";
            }
            else if (timeDifference.TotalDays >= 1)
            {
                result = $"{(int)timeDifference.TotalHours / 24}d {(int)timeDifference.TotalHours % 24}h ago";
            }
            else if (timeDifference.TotalHours >= 1)
            {
                result = $"{(int)timeDifference.TotalHours}h {(int)timeDifference.TotalMinutes % 60}m ago";
            }
            else if (timeDifference.TotalMinutes >= 1)
            {
                result = $"{(int)timeDifference.TotalMinutes}m ago";
            }
            else
            {
                result = "now";
            }
        }

        return result;
    }
    /// <summary>
    /// A method displays popup window with warning message if the user has not provided any title for a new acitivity
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    private async void ShowWarningMessage(string title, string message)
    {
        await DisplayAlert(title, message, "OK");
    }

    /// <summary>
    /// This method saves data of (Activity) class
    /// </summary>
    public void SaveEventsData()
    {
        jsonString = JsonConvert.SerializeObject(activitiesList);
        try
        {
            var localFolder = FileSystem.Current.AppDataDirectory;
            var filePath = Path.Combine(localFolder, fileName);
            Debug.WriteLine(filePath);
            File.WriteAllText(filePath, jsonString);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    ///This method loads data from .txt file that contains each (Activity) objects after which updates timeDifference property to see how much time has passed since last opening -> and saves new data back straight away 
    /// </summary>
    public void LoadEventsData()
    {
        try
        {
            var localFolder = FileSystem.Current.AppDataDirectory;
            var filePath = Path.Combine(localFolder, fileName);
            jsonString = File.ReadAllText(filePath);
            activitiesList = JsonConvert.DeserializeObject<ObservableCollection<Activity>>(jsonString);

            //now after we get data we need to update information as some time could have passed since last opening
            UpdateTimeDifference();

            SaveEventsData();
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            activitiesIdCount = 0;
        }
    }

    /// <summary>
    /// This method saves data of (ActivityHistory) class
    /// </summary>
    public void SaveEventsHistoryData()
    {
        jsonString2 = JsonConvert.SerializeObject(activitiesHistoryList);
        try
        {
            var localFolder = FileSystem.Current.AppDataDirectory;
            var filePath = Path.Combine(localFolder, fileName2);
            Debug.WriteLine(filePath);
            File.WriteAllText(filePath, jsonString2);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// This method loads data from .txt file that contains each (ActivityHistory) objects after which updates starColor property of that class to mark each history as important or not important activity| green means that an event has been created no longer than one
    /// day. Yellow - no longer than a month. And the rest will be black. All data saves straight away
    /// </summary>
    public void LoadEventsHistoryData()
    {
        try
        {
            var localFolder = FileSystem.Current.AppDataDirectory;
            var filePath = Path.Combine(localFolder, fileName2);
            jsonString2 = File.ReadAllText(filePath);
            activitiesHistoryList = JsonConvert.DeserializeObject<ObservableCollection<ObservableCollection<ActivityHistory>>>(jsonString2);

            //now after we get data we need to update information as some time could have passed since last opening
            UpdateStarColors();

            SaveEventsHistoryData();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    /// <summary>
    ///A method that runs through each activity at a time and passes object's property selectedTime to calculate time difference and update timeDifference value based on selectedTime
    /// </summary>
    public void UpdateTimeDifference()
    {
        foreach (Activity activity in activitiesList)
        {
            activity.timeDifference = CalculateTimeDifference(activity.selectedDateTime);
        }
    }

    /// <summary>
    /// A method that updates star color based on when last time object of type (ActivityHistory) was created
    /// </summary>
    public void UpdateStarColors()
    {
        foreach (ObservableCollection<ActivityHistory> activites in activitiesHistoryList)
        {
            foreach (ActivityHistory activityHistory in activites)
            {
                if ((currentSystemDateTime - activityHistory.lastTimeModified).TotalDays < 1)
                {
                    activityHistory.starColor = "green_star.png";
                }
                else if ((currentSystemDateTime - activityHistory.lastTimeModified).TotalDays < 30)
                {
                    activityHistory.starColor = "yellow_star.png";
                }
                else
                {
                    activityHistory.starColor = "black_star.png";
                }
            }
        }
    }

    /// <summary>
    /// A method loads preferenced -> amount of existing acitvities
    /// </summary>
    public void LoadIdPreferences()
    {
        activitiesIdCount = Preferences.Default.Get("activitiesIdCount", activitiesIdCount);
    }

    /// <summary>
    /// A method updates colors and font of labels, buttons, date time pickers
    /// </summary>
    public void UpdateNewEventPageDesign()
    {
        settingsPage.LoadSettingsPreferences();

        for (int i = 0; i < labels.Count; i++)
        {
            labels[i].TextColor = Color.FromArgb(settingsPage.colors[settingsPage.colorIndex % settingsPage.colors.Count]);
            labels[i].FontSize = settingsPage.fonts[settingsPage.fontIndex % settingsPage.fonts.Count];
        }
        backButton.TextColor = Color.FromArgb(settingsPage.colors[settingsPage.colorIndex % settingsPage.colors.Count]);
        backButton.FontSize = settingsPage.fonts[settingsPage.fontIndex % settingsPage.fonts.Count];

        titleEditor.TextColor = Color.FromArgb(settingsPage.colors[settingsPage.colorIndex % settingsPage.colors.Count]);
        titleEditor.FontSize = settingsPage.fonts[settingsPage.fontIndex % settingsPage.fonts.Count];

        descriptionEditor.TextColor = Color.FromArgb(settingsPage.colors[settingsPage.colorIndex % settingsPage.colors.Count]);
        descriptionEditor.FontSize = settingsPage.fonts[settingsPage.fontIndex % settingsPage.fonts.Count];

        myDatePicker.TextColor = Color.FromArgb(settingsPage.colors[settingsPage.colorIndex % settingsPage.colors.Count]);
        myDatePicker.FontSize = settingsPage.fonts[settingsPage.fontIndex % settingsPage.fonts.Count];

        myTimePicker.TextColor = Color.FromArgb(settingsPage.colors[settingsPage.colorIndex % settingsPage.colors.Count]);
        myTimePicker.FontSize = settingsPage.fonts[settingsPage.fontIndex % settingsPage.fonts.Count];
    }

}


/// <summary>
/// The Activity class represents a user's activity with associated details.
/// </summary>
public class Activity
{
    //Gets or sets the icon name associated with the activity.
    public string iconName { get; set; }
    //Gets or sets the date and time when the activity was selected or created.
    public DateTime selectedDateTime { get; set; }
    // Gets or sets the title of the activity.
    public string title { get; set; }
    //Gets or sets the time difference or duration related to the activity.
    public string timeDifference { get; set; }
    //Gets or sets a unique identifier for the activity.
    public int Id { get; set; }

}

/// <summary>
/// The ActivityHistory class stores historical data related to a user's activity.
/// </summary>
public class ActivityHistory
{
    //Gets or sets the date and time when the activity's information was last created.
    public DateTime lastTimeModified { get; set; }
    //Gets or sets the color associated with the activity for visual highlighting (e.g., star color).
    public string starColor { get; set; }
    //Gets or sets a description or additional information about the activity's history.
    public string description { get; set; }
}