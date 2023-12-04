using CommunityToolkit.Maui.Core.Extensions;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace HowLongSince_AndrewLukashchuk
{
    /// <summary>
    /// Class MainPage represents the main page where the user can sort their activities by day, week, or all. 
    /// Can also create a new activity, go to settings, or simply keep all your to-dos on one page
    /// </summary>
    public partial class MainPage : ContentPage
    {
        //Creating a list to pass activities to that list to alter it without modifying an original one from newEvent class
        public ObservableCollection<Activity> allActivitiesList = new ObservableCollection<Activity>();
        //Current system date and time to compare this value with provided user's date time
        public DateTime currentDateAndTime = DateTime.Now;

        SettingsPage settingsPage = new SettingsPage();

        public MainPage()
        {
            InitializeComponent();
        }

       /// <summary>
       /// Button addNewEvent after being clicked opens a new page of type newEvent where user can create a new activity
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private async void addNewEvent_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewEvent()); 
        }

        /// <summary>
        /// Button setting after being clicked opens a new page of type SettingsPage where user can change app setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DisplayEvents();
            DisplayActivityCountAll();
            UpdateMainPageDesign();
        }

        /// <summary>
        /// This method loads two json files with two objects of type Activity and ActivityHistory to display the user's 
        /// information about registered activities.First, we load files -> then we copy all data from the list with activities 
        /// from newEvent class and send it to the list of main page class, sort the list by date(earliest to latest)
        /// -> bind the main page's list with listview to display data
        /// </summary>
        public void DisplayEvents()
        {
            //Creating an instance of newEvent data type to get access to this class methods and properties
            NewEvent newEvent = new NewEvent();

            newEvent.LoadEventsData();
            newEvent.LoadEventsHistoryData();

            allActivitiesList = newEvent.activitiesList;

            //ordering data by latest datetime using LINQ
            allActivitiesList = allActivitiesList.OrderByDescending(item => item.selectedDateTime).ToObservableCollection();
            eventsView.ItemsSource = allActivitiesList;
        }

        /// <summary>
        /// This method counts how many activities user created in total
        /// </summary>
        public void DisplayActivityCountAll()
        {
            int countActivity = 0;
            foreach (Activity activity in allActivitiesList)
            {
                countActivity++;
            }

            labelActivity.Text = countActivity.ToString();
        }

        /// <summary>
        /// This method displays specific activities based on their date
        /// </summary>
        /// <param name="temporaryListWithActivities"></param>
        public void DisplayChosenActivities(ObservableCollection<Activity> temporaryListWithActivities)
        {
            int countActivity = 0;
            foreach (Activity activity in temporaryListWithActivities)
            {
                countActivity++;
            }

            labelActivity.Text = countActivity.ToString();
        }

        /// <summary>
        /// This method separates activities based on their date. For that purpose temporary list is being created that 
        /// will save those activities. Then gets an index of the picker to determine which option the user has chosen. 
        /// If 0 -> all activities are being shown | 1 -> activities that do not extend a year difference in time | 
        /// 2 -> activities that do not extend more than 1-day time difference
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pickerViewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ObservableCollection<Activity> temporaryActivitiesList = new ObservableCollection<Activity>();

            int selectedIndex = pickerViewMode.SelectedIndex;

            TimeSpan timeDifference;

            //clearing list to get new activites later on
            temporaryActivitiesList.Clear();

            if (selectedIndex == 0)
            {
                temporaryActivitiesList = allActivitiesList;
                eventsView.ItemsSource = temporaryActivitiesList;
            }
            else if (selectedIndex == 1)
            {
                foreach (Activity item in allActivitiesList)
                {
                    timeDifference = currentDateAndTime - item.selectedDateTime;
                    if (timeDifference.TotalDays < 365)
                    {
                        temporaryActivitiesList.Add(item);
                    }
                }
                eventsView.ItemsSource = temporaryActivitiesList;
            }
            else
            {
                 foreach (Activity item in allActivitiesList)
                {
                    timeDifference = currentDateAndTime - item.selectedDateTime;
                    if (timeDifference.TotalDays < 1)
                    {
                        temporaryActivitiesList.Add(item);
                    }
                }
                eventsView.ItemsSource = temporaryActivitiesList;
            }
            DisplayChosenActivities(temporaryActivitiesList);
        }

        /// <summary>
        /// This method opens the page with an activity that is being tapped. Then gets the id for a chosen activity to alter the activity's 
        ///setting later on based on that id.Id sent to class ActivityPage where id can be used as identification chosen activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventsView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Activity selectedItem)
            {
                var activityId = selectedItem.Id;

                Navigation.PushAsync(new ActivityPage(activityId));
            }
        }

        /// <summary>
        /// A method updates colors and font of labels, buttons, date time pickers
        /// </summary>
        public void UpdateMainPageDesign()
        {
            settingsPage.LoadSettingsPreferences();
            //user label
            userMainPageLabel.Text = settingsPage.userNickname;
            userMainPageLabel.TextColor = Color.FromArgb(settingsPage.colors[settingsPage.colorIndex % settingsPage.colors.Count]);
            userMainPageLabel.FontSize = settingsPage.fonts[settingsPage.fontIndex % settingsPage.fonts.Count];

            //activity count label
            labelActivity.TextColor = Color.FromArgb(settingsPage.colors[settingsPage.colorIndex % settingsPage.colors.Count]);
            labelActivity.FontSize = settingsPage.fonts[settingsPage.fontIndex % settingsPage.fonts.Count];

            //picker view
            pickerViewMode.BackgroundColor = Color.FromArgb(settingsPage.colors[settingsPage.colorIndex % settingsPage.colors.Count]);
            pickerViewMode.FontSize = settingsPage.fonts[settingsPage.fontIndex % settingsPage.fonts.Count];

            //events view
            
        }
    }
}