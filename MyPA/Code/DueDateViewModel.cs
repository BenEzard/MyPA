using MyPA.Code.Data.Models;
using MyPA.Code.Data.Services;
using MyPA.Code.UI.Util;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyPA.Code
{
    public class DueDateViewModel : BaseViewModel
    {

        /// <summary>
        /// The Repository handles all of the data collection; editing and deleting methods.
        /// </summary>
        private IWorkItemRepository workItemRepository = new WorkItemRepository();

        public Action CloseAction { get; set; }

        private WorkItemDueDate _originalData;
        public WorkItemDueDate OriginalData
        {
            get => _originalData;
            set
            {
                _originalData = value;
                CurrentSelectedDate = value.DueDateTime;
                int hour = value.DueDateTime.Hour;
                SelectedHourString = hour.ToString().PadLeft(2, '0');
                int minute = value.DueDateTime.Minute;
                SelectedMinuteString = minute.ToString().PadLeft(2, '0');
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// The current selected date and time.
        /// </summary>
        public DateTime _currentSelectedDate;
        public DateTime CurrentSelectedDate
        {
            get => _currentSelectedDate;
            set
            {
                _currentSelectedDate = value;
                GenerateDueDateTime();
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// The object which is created with the new data.
        /// </summary>
        public WorkItemDueDate NewDueDate;

        /// <summary>
        /// The list of possible hours that can be selected. In 24 hour format.
        /// Populated based on START_OF_BUSINESS_DAY and END_OF_BUSINESS_DAY preferences.
        /// </summary>
        private List<string> _hourSelectionList = new List<string>();
        public List<string> HourSelectionList
        {
            get => _hourSelectionList;
            set
            {
                _hourSelectionList = value;
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// The list of possible minute options that can be selected.
        /// Populated based on DUE_DATE_MINUTE_INCREMENTS preference between 0 and 60.
        /// </summary>
        private List<string> _minuteSelectionList = new List<string>();
        public List<string> MinuteSelectionList
        {
            get => _minuteSelectionList;
            set
            {
                _minuteSelectionList = value;
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// The DueDate hour that has been selected (as an int).
        /// </summary>
        private int _selectedHour;
        public int SelectedHour
        {
            get
            {
                if (SelectedHourString != null)
                    return Int32.Parse(SelectedHourString);
                else
                    return 0;
            }

            set
            {
                _selectedHour = value;
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// The DueDate minute that has been selected (as an int).
        /// </summary>
        private int _selectedMinute;
        public int SelectedMinute
        {
            get
            {
                if (SelectedMinuteString != null)
                    return Int32.Parse(SelectedMinuteString);
                else
                    return 0;
            }
            set
            {
                _selectedMinute = value;
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// The selected DueDate hour (as a string)
        /// </summary>
        private string _selectedHourString;
        public string SelectedHourString { 
            get => _selectedHourString;
            set
            {
                _selectedHourString = value;
                GenerateDueDateTime();
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// The selected DueDate minute (as a string)
        /// </summary>
        private string _selectedMinuteString;
        public string SelectedMinuteString
        {
            get => _selectedMinuteString;
            set
            {
                _selectedMinuteString = value;
                GenerateDueDateTime();
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// Get the Change Reason (why the DueDate was changed).
        /// </summary>
        private string _changeReason;
        public string ChangeReason
        {
            get => _changeReason;
            set
            {
                _changeReason = value;
                OnPropertyChanged("");
            }
        }

        public DueDateViewModel()
        {
            Preferences = new BaseRepository().GetPreferences("DueDate");

            HourSelectionList = CollectionMethods.GenerateIncrementalStringList(GetAppPreferenceValueAsInt(PreferenceName.START_OF_BUSINESS_DAY),
                GetAppPreferenceValueAsInt(PreferenceName.END_OF_BUSINESS_DAY), 1);

            // Calculate the upper range of the MINUTES option.
            int increments = GetAppPreferenceValueAsInt(PreferenceName.DUE_DATE_MINUTE_INCREMENTS);
            MinuteSelectionList = CollectionMethods.GenerateIncrementalStringList(0, (60 - increments), increments);

            CalculateNonSelectableDates();

        }

        /// <summary>
        /// Get the list of non-selectable dates.
        /// </summary>
        private List<DateTime> _nonSelectableDates = new List<DateTime>();
        public List<DateTime> NonSelectableDates
        {
            get => _nonSelectableDates;
            set
            {
                _nonSelectableDates = value;
            }
        }

        /// <summary>
        /// Convert the List<DateTime> into List<CalendarDateTime>
        /// </summary>
        /// <returns></returns>
        public List<CalendarDateRange> GetNonSelectableDatesAsCalendarDateRange()
        {
            List<CalendarDateRange> rValue = new List<CalendarDateRange>();
            foreach (DateTime dt in NonSelectableDates)
            {
                rValue.Add(new CalendarDateRange(dt));
            }
            return rValue;
        }

        /// <summary>
        /// Get the list of non selectable dates for the calendar.
        /// TODO Could be improved using this code: https://stackoverflow.com/questions/1638128/how-to-bind-blackoutdates-in-wpf-toolkit-calendar-control
        /// TODO Could change the Saturday and Sunday code into a Saturday-and-Sunday (reducing the number of loops and data items in List<>, but meh...
        /// </summary>
        /// <returns></returns>
        private void CalculateNonSelectableDates()
        {
            List<DateTime> dates = new List<DateTime>();
            int blackoutRange = GetAppPreferenceValueAsInt(PreferenceName.DUE_DATE_BLACKOUT_RANGE);

            // For me, the StartOfWeek is a Sunday.
            DateTime StartOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            if (GetAppPreferenceValueAsBool(PreferenceName.DUE_DATE_CAN_BE_SUNDAY) == false)
            {
                dates.AddRange(GetWeeklyIterations(StartOfWeek, blackoutRange));
            }

            // Monday
            var firstMonday = StartOfWeek.AddDays(1);
            if (GetAppPreferenceValueAsBool(PreferenceName.DUE_DATE_CAN_BE_MONDAY) == false)
            {
                dates.AddRange(GetWeeklyIterations(firstMonday, blackoutRange));
            }

            // Tuesday
            var firstTuesday = StartOfWeek.AddDays(2);
            if (GetAppPreferenceValueAsBool(PreferenceName.DUE_DATE_CAN_BE_TUESDAY) == false)
            {
                dates.AddRange(GetWeeklyIterations(firstTuesday, blackoutRange));
            }

            // Wednesday
            var firstWednesday = StartOfWeek.AddDays(3);
            if (GetAppPreferenceValueAsBool(PreferenceName.DUE_DATE_CAN_BE_WEDNESDAY) == false)
            {
                dates.AddRange(GetWeeklyIterations(firstWednesday, blackoutRange));
            }

            // Thursday
            var firstThursday = StartOfWeek.AddDays(4);
            if (GetAppPreferenceValueAsBool(PreferenceName.DUE_DATE_CAN_BE_THURSDAY) == false)
            {
                dates.AddRange(GetWeeklyIterations(firstThursday, blackoutRange));
            }

            // Friday
            var firstFriday = StartOfWeek.AddDays(5);
            if (GetAppPreferenceValueAsBool(PreferenceName.DUE_DATE_CAN_BE_FRIDAY) == false)
            {
                dates.AddRange(GetWeeklyIterations(firstFriday, blackoutRange));
            }

            // Saturday
            var firstSaturday = StartOfWeek.AddDays(6);
            if (GetAppPreferenceValueAsBool(PreferenceName.DUE_DATE_CAN_BE_SATURDAY) == false)
            {
                dates.AddRange(GetWeeklyIterations(firstSaturday, blackoutRange));
            }

            dates.AddRange(workItemRepository.GetIneligibleDueDates());

            NonSelectableDates = dates;
        }

        /// <summary>
        /// Get a list of date records, starting from startDate for the specified number of iterations.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="numberOfIterations"></param>
        /// <returns></returns>
        private List<DateTime> GetWeeklyIterations(DateTime startDate, int numberOfIterations)
        {
            List<DateTime> dates = new List<DateTime>();
            DateTime theDate = startDate;

            for (int i = 0; i <= numberOfIterations; i++)
            {
                dates.Add(theDate);
                theDate = theDate.AddDays(7);
            }
            return dates;
        }

        /// <summary>
        /// Checks to see if a specified Date is acceptable (i.e. not on the blackout list). If it isn't, returns the specified date, otherwise increments until the first acceptable date.
        /// </summary>
        /// <param name="testDate"></param>
        /// <returns></returns>
        public DateTime GetNextAcceptableDate(DateTime testDate)
        {
            DateTime rValue = testDate;

            while (_nonSelectableDates.Contains(rValue.Date)) {
                rValue = rValue.AddDays(1);
            }
            return rValue;
        }

        /// <summary>
        /// Using the values in the dialog (Calendar, SelectedHour and SelectedMinute) , creates a DateTime value.
        /// </summary>
        private void GenerateDueDateTime()
        {
            DateTime dateTime = GetNextAcceptableDate(_currentSelectedDate).Date;
            dateTime = dateTime.AddHours(SelectedHour);
            dateTime = dateTime.AddMinutes((double)SelectedMinute);
            _currentSelectedDate = dateTime;
        }

        RelayCommand _applyDueDateChangeCommand;
        public ICommand ApplyDueDateChangeCommand
        {
            get
            {
                if (_applyDueDateChangeCommand == null)
                {
                    _applyDueDateChangeCommand = new RelayCommand(SaveDueDateChange, DueDateReadyForSave);
                }
                return _applyDueDateChangeCommand;
            }
        }

        /// <summary>
        /// Save a new WorkItem to the database and add it to the WorkItem collection.
        /// </summary>
        public void SaveDueDateChange()
        {
            NewDueDate = new WorkItemDueDate(_originalData.WorkItemID, _currentSelectedDate, _changeReason);

            int dueDateGracePeriod = GetAppPreferenceValueAsInt(PreferenceName.DUE_DATE_SET_WINDOW_SECONDS);

            // Check the length of time between this and the last DueDate change.
            double secondsSinceChange = (NewDueDate.CreationDateTime - _originalData.CreationDateTime).TotalSeconds;
            if (secondsSinceChange <= dueDateGracePeriod)
            {
                workItemRepository.UpdateWorkItemDueDate(NewDueDate);
            }
            else
            {
                workItemRepository.InsertWorkItemDueDate(NewDueDate);
            }

            Messenger.Default.Send(NewDueDate);
        }

        /// <summary>
        /// Is the dialog ready to be saved?
        /// </summary>
        /// <returns></returns>
        public bool DueDateReadyForSave()
        {
            if ((GetAppPreferenceValueAsBool(PreferenceName.DUE_DATE_REQUIRE_CHANGE_REASON))
                && ((ChangeReason == null) || (ChangeReason == "")))
                return false;
            else
                return true;
        }

/*        RelayCommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(Close, CanClose);
                }
                return _closeCommand;
            }
        }

        /// <summary>
        /// Save a new WorkItem to the database and add it to the WorkItem collection.
        /// </summary>
        public void Close()
        {
            this.Close();
        }

        /// <summary>
        /// Is the dialog ready to be closed?
        /// </summary>
        /// <returns></returns>
        public bool CanClose()
        {
            return true;
        }*/
    }
}
