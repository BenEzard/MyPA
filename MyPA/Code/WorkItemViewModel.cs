using MyPA.Code.Data.Services;
using MyPA.Code.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MyPA.Code.UI.Util;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using MyPA.Code.Util;
using MyPA.Code.Data.Actions;
using MyPA.Code.Data.Events;
using MyPA.Code.Data;

namespace MyPA.Code
{
    public class WorkItemViewModel : BaseViewModel
    {
        private static string TAB_NAME = "TabTaskDescription";

        /// <summary>
        /// The Repository handles all of the data collection; editing and deleting methods.
        /// </summary>
        private IWorkItemRepository workItemRepository = new WorkItemRepository();

        /// <summary>
        /// An ObservableCollection of all 'active' WorkItems.
        /// (Potentially this will change to be all work items, if I can work out how to filter the data well).
        /// </summary>
        public ObservableCollection<WorkItem> WorkItems { get; set; } = new ObservableCollection<WorkItem>();

        /// <summary>
        /// A list of all WorkItemStatuses.
        /// This is to populate the UI, and is NOT related to any particular WorkItem.
        /// </summary>
        public List<WorkItemStatus> WorkItemStatuses { get; set; }

        public List<WorkItemStatusFilter> WorkItemStatusFilter { get; set; } = new List<WorkItemStatusFilter>();

        public delegate void ModelEventHandler(object obj, WorkItemJournalEvent e);
        public event ModelEventHandler ModelEvent;


        /// <summary>
        /// Return a WorkItemStatus by the given WorkItemStatusID
        /// </summary>
        /// <param name="workItemStatusID"></param>
        /// <returns></returns>
        private WorkItemStatus GetWorkItemStatus(int workItemStatusID)
        {
            WorkItemStatus rValue = null;
            foreach (WorkItemStatus wis in WorkItemStatuses)
            {
                if (wis.WorkItemStatusID == workItemStatusID)
                    rValue = wis;
            }
            return rValue;
        }

        private WorkItem _selectedWorkItem = null;
        /// <summary>
        /// The currently selected WorkItem.
        /// </summary>
        public WorkItem SelectedWorkItem
        {
            get => _selectedWorkItem;
            set
            {
                if (value == null)
                    return;

                UpdateWorkItem(_selectedWorkItem);
                _selectedWorkItem = value;
                Messenger.Default.Send(new WorkItemSelectedAction(_selectedWorkItem));
                Messenger.Default.Send(new WorkItemJournalEvent(WorkItemJournalAction.MOVE_VERTICAL_SPLIT, UI.SplitSetting.EQUAL_SPLIT));
                OnPropertyChanged("");
            }
        }

        public void UpdateWorkItem(WorkItem workItem)
        {
            if (workItem != null)
            {
                workItemRepository.UpdateWorkItem(workItem);
            }
        }

        /// <summary>
        /// A VM variable mirroring SelectedWorkItem.Title; bound by the view.
        /// </summary>
        public string Title {
            get
            {
                if (_selectedWorkItem != null)
                    return _selectedWorkItem.Title;
                else 
                    return "";
            }
            set {
                _selectedWorkItem.Title = value;
                OnPropertyChanged("");
                _workItemOverview.View.Refresh();
            }
        }

        /// <summary>
        /// A VM variable mirroring SelectedWorkItem.Description; bound by the view.
        /// </summary>
        public string TaskDescription
        {
            get
            {
                string rValue = "";
                if (_selectedWorkItem != null)
                    rValue = _selectedWorkItem.Description;
                return rValue;
            }
            set
            {
                _selectedWorkItem.Description = value;
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// Return the maximum Modified DateTime for the selected Work Item, analysing all aspects of the WorkItem.
        /// </summary>
        public DateTime? SelectedWorkItemModifiedDateTime
        {
            get
            {
                if (_selectedWorkItem == null)
                    return null;

                List<DateTime?> dates = new List<DateTime?>();
                if (_selectedWorkItem.CreationDateTime.HasValue)
                    dates.Add(_selectedWorkItem.CreationDateTime);
                if (_selectedWorkItem.ModificationDateTime.HasValue)
                    dates.Add(_selectedWorkItem.ModificationDateTime);
                if (_selectedWorkItem.CurrentWorkItemDueDate != null)
                {
                    if (_selectedWorkItem.CurrentWorkItemDueDate.CreationDateTime.HasValue)
                        dates.Add(_selectedWorkItem.CurrentWorkItemDueDate.CreationDateTime);
                    if (_selectedWorkItem.CurrentWorkItemDueDate.ModificationDateTime.HasValue)
                        dates.Add(_selectedWorkItem.CurrentWorkItemDueDate.ModificationDateTime);
                }
                if (_selectedWorkItem.CurrentWorkItemStatusEntry != null)
                {
                    if (_selectedWorkItem.CurrentWorkItemStatusEntry.CreationDateTime.HasValue)
                        dates.Add(_selectedWorkItem.CurrentWorkItemStatusEntry.CreationDateTime);
                    if (_selectedWorkItem.CurrentWorkItemStatusEntry.ModificationDateTime.HasValue)
                        dates.Add(_selectedWorkItem.CurrentWorkItemStatusEntry.ModificationDateTime);
                }
                return dates.Max();
            }
        }

        /// <summary>
        /// List of UI tabs that are available on the UI.
        /// </summary>
        private List<string> _listOfUITabs = new List<string>();
        public void RegisterUITab(string tabName)
        {
            _listOfUITabs.Add(tabName.ToUpper());
        }


        /// <summary>
        /// Returns true if a WorkItem has been selected; otherwise false
        /// </summary>
        public bool IsWorkItemSelected
        {
            get
            {
                if (_selectedWorkItem == null)
                    return false;
                else
                    return true;
            }
        }

        public bool IsAWorkItemSelected()
        {
            return IsWorkItemSelected;
        }

        public int SelectedWorkItemCompletion
        {
            get
            {
                int rValue = 0;

                if (_selectedWorkItem != null)
                {
                    if ((IsApplicationInAddMode) || (_selectedWorkItem.WorkItemStatusEntryCount == 0))
                    {
                        rValue = 0;
                    }
                    else 
                        rValue = _selectedWorkItem.GetLastWorkItemStatusEntry().CompletionAmount;
                }

                return rValue;
            }
            set
            {
                // Create a new WorkItemStatusEntry to record the change in the completion amount.
                var wise = GenerateNewWorkItemStatusEntry(value);

                // Check to see if the latest WISE was created within the last x seconds; if so, update instead of insert.
                WorkItemStatusEntry lastItem = _selectedWorkItem.CurrentWorkItemStatusEntry;
                double secondsDifferent = (DateTime.Now - lastItem.CreationDateTime.Value).TotalSeconds;
                double period = GetAppPreferenceValueAsDouble(PreferenceName.WORK_ITEM_STATUS_SET_WINDOW_SECONDS);
                if (secondsDifferent < period)
                {
                    workItemRepository.UpdateWorkItemStatusEntry(wise);
                    // Copy the values over (update the existing item instead of recreating).
                    lastItem.StatusLabel = wise.StatusLabel;
                    lastItem.CompletionAmount = wise.CompletionAmount;
                    lastItem.WorkItemStatusID = wise.WorkItemStatusID;
                    _selectedWorkItem.CurrentWorkItemStatusEntry = lastItem;
                }
                else
                {
                    workItemRepository.InsertWorkItemStatusEntry(wise);
                    _selectedWorkItem.CurrentWorkItemStatusEntry = wise;
                }
                OnPropertyChanged("");
            }
        }

        private int _selectedWorkItemTabIndex;
        public int SelectedWorkItemTabIndex
        {
            get => _selectedWorkItemTabIndex;
            set
            {
                _selectedWorkItemTabIndex = value;
                OnPropertyChanged("");
            }
        }

        public WorkItemStatus SelectedWorkItemStatus
        {
            get
            {
                WorkItemStatus rValue = null;
                
                if (_selectedWorkItem != null)
                {
                    if ((IsApplicationInAddMode) || (_selectedWorkItem.WorkItemStatusEntryCount == 0))
                        rValue = GetWorkItemStatuses(true, true).ToList()[0];
                    else
                        rValue = GetWorkItemStatus(_selectedWorkItem.GetLastWorkItemStatusEntry().WorkItemStatusID);
                }

                return rValue;
            }
            set
            {
                ChangeWorkItemStatus(GetWorkItemStatus(_selectedWorkItem.GetLastWorkItemStatusEntry().WorkItemStatusID), value);
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// WorkItemViewModel constructor.
        /// </summary>
        public WorkItemViewModel()
        {
            ApplicationViewModel appViewModel = new ApplicationViewModel();

            // Load Preferences
            Preferences = workItemRepository.GetWorkItemPreferences();

            Messenger.Default.Register<AppAction>(this, AppActionReceived);
            Messenger.Default.Register<WorkItemCreatingAction>(this, WorkItemCreatingNotification);
            Messenger.Default.Register<WorkItemDueDate>(this, ReceivedWorkItemDueDateChange);
            Messenger.Default.Register<WorkItemSelectTabAction>(this, ChangeTabSelection);
/*            Messenger.Default.Register<WorkItemJournalCreatingAction>(this, WorkItemJournalCreatingNotification);
            Messenger.Default.Register<WorkItemSaveCommand>(this, SaveCommandNotification);*/

            LoadWorkItemStatuses();

            LoadWorkItems();

            _workItemOverview = new CollectionViewSource();
            _workItemOverview.Source = WorkItems;
            _workItemOverview.Filter += WorkItemOverviewFilter;

            OnPropertyChanged("");
        }

     /*   private void SaveCommandNotification(WorkItemSaveCommand action)
        {
            SaveCommand.ButtonText = action.ButtonText;
            SaveCommand.ButtonImagePath = action.ButtonImagePath;
            SaveCommand.CommandAction = action.CommandAction;
        }

        private void WorkItemJournalCreatingNotification(WorkItemJournalCreatingAction action)
        {
            SaveCommand.ButtonText = "New Journal Entry";
            SaveCommand.ButtonImagePath = "";
//            SaveCommand.CommandAction = WorkItemJournalCreatingCommand;
        }*/

        private void ChangeTabSelection(WorkItemSelectTabAction action)
        {
            SelectedWorkItemTabIndex = GetUITabIndex(action.Name);
        }

        /// <summary>
        /// Generate a new WorkItemStatusEntry by taking the current WorkItemStatus and CompletionAmount
        /// </summary>
        /// <param name="completionAmount"></param>
        /// <returns></returns>
        private WorkItemStatusEntry GenerateNewWorkItemStatusEntry(int completionAmount)
        {
            var currentWIS = GetWorkItemStatus(_selectedWorkItem.GetLastWorkItemStatusEntry().WorkItemStatusID);
            var wise = new WorkItemStatusEntry(_selectedWorkItem.WorkItemID.Value, currentWIS.WorkItemStatusID, completionAmount, currentWIS.StatusLabel);
            return wise;
        }

        /// <summary>
        /// Process an AppAction that has been received.
        /// </summary>
        /// <param name="action"></param>
        private void AppActionReceived(AppAction action)
        {
            switch (action)
            {
                case AppAction.APPLICATION_CLOSING:
                    UpdateWorkItem(_selectedWorkItem);
                    break;
                case AppAction.DELETING_WORK_ITEM:
                    DeleteWorkItem(_selectedWorkItem);
                    break;
            }
        }

        /// <summary>
        /// Return the tab index based on the specified tooltip.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int GetUITabIndex(string name)
        {
            name = name.ToUpper();
            int rValue = -1;
            for (int i = 0; i < _listOfUITabs.Count; i++)
            {
                if (_listOfUITabs[i].Equals(name))
                {
                    rValue = i;
                    break;
                }
            }
            return rValue;
        }

        /// <summary>
        /// Begin the creation of a new Work Item. 
        /// (Note that the WorkItem is not saved at this time).
        /// </summary>
        private void WorkItemCreatingNotification(WorkItemCreatingAction a)
        {
            // Set the application into ADD_MODE.
            AppMode = ApplicationMode.ADD_MODE;
/*            SaveCommand.ButtonText = "Save";
            SaveCommand.ButtonImagePath = "../../Images/report_add.png";
            SaveCommand.CommandAction = WorkItemSaveNewCommand;*/

            // Change the WorkItem tab that is currently being displayed.
            SelectedWorkItemTabIndex = GetUITabIndex(TAB_NAME);

            SelectedWorkItem = new WorkItem();
            DateTime newDueDate = new DueDateViewModel().GenerateDefaultDueDate();
            var widd = new WorkItemDueDate(newDueDate, "Initial WorkItem creation.");
            SelectedWorkItem.CurrentWorkItemDueDate = widd; // Not saved yet because no WorkItemID
        }

        /// <summary>
        /// Process a WorkItemDueDate change.
        /// </summary>
        /// <param name="wiDueDate"></param>
        private void ReceivedWorkItemDueDateChange(WorkItemDueDate wiDueDate)
        {
            WorkItemDueDate _originalData = _selectedWorkItem.CurrentWorkItemDueDate;

            int dueDateGracePeriod = GetAppPreferenceValueAsInt(PreferenceName.DUE_DATE_SET_WINDOW_SECONDS);

            // Check the length of time between this and the last DueDate change.
            double secondsSinceChange = (wiDueDate.CreationDateTime.Value - _originalData.CreationDateTime.Value).TotalSeconds;
            if (secondsSinceChange <= dueDateGracePeriod)
            {
                workItemRepository.UpdateWorkItemDueDate(wiDueDate);
            }
            else
            {
                workItemRepository.InsertWorkItemDueDate(wiDueDate);
            }

            SelectedWorkItem.CurrentWorkItemDueDate = wiDueDate;
        }

        /// <summary>
        /// Loads all of the WorkItems from the repository into the ActiveWorkItems collection.
        /// </summary>
        private void LoadWorkItems()
        {
            foreach (WorkItem wi in workItemRepository.GetWorkItems(GetAppPreferenceValueAsInt(PreferenceName.LOAD_STALE_DAYS)))
            {
                WorkItems.Add(wi);
            }
        }

        /// <summary>
        /// Change the WorkItemStatus.
        /// Note that the SelectedItem implementation on the ComboBox doesn't fire unless there has been a change. 
        ///     (i.e. the same object being selected again doesn't change it.)
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private void ChangeWorkItemStatus(WorkItemStatus oldValue, WorkItemStatus newValue)
        {
            // If the old value is a Closed status and the new value is an opened status, then set completion amount to PreferenceName.STATUS_COMPLETE_TO_ACTIVE
            if ((oldValue.IsConsideredActive == false) && (newValue.IsConsideredActive == true))
            {
                SelectedWorkItemCompletion = GetAppPreferenceValueAsInt(PreferenceName.STATUS_COMPLETE_TO_ACTIVE);
            }

            // If it is changed to the default closed status, then set completion to 100%
            if ((newValue.IsConsideredActive == false) && (newValue.IsDefault == true))
            {
                SelectedWorkItemCompletion = 100;
            }

            var wise = new WorkItemStatusEntry(_selectedWorkItem.WorkItemID.Value, newValue.WorkItemStatusID, SelectedWorkItemCompletion, newValue.StatusLabel);

            // Check to see if the latest WISE was created within the last x seconds; if so, update instead of insert.
            WorkItemStatusEntry lastItem = _selectedWorkItem.CurrentWorkItemStatusEntry;
            double secondsDifferent = (DateTime.Now - lastItem.CreationDateTime.Value).TotalSeconds;
            double period = GetAppPreferenceValueAsDouble(PreferenceName.WORK_ITEM_STATUS_SET_WINDOW_SECONDS);
            if (secondsDifferent <= period)
            {
                workItemRepository.UpdateWorkItemStatusEntry(wise);
                // Copy the values over (update the existing item instead of recreating).
                lastItem.StatusLabel = wise.StatusLabel;
                lastItem.CompletionAmount = wise.CompletionAmount;
                lastItem.WorkItemStatusID = wise.WorkItemStatusID;
                _selectedWorkItem.CurrentWorkItemStatusEntry = lastItem;
            }
            else
            {
                workItemRepository.InsertWorkItemStatusEntry(wise);
                _selectedWorkItem.CurrentWorkItemStatusEntry = wise;
            }
        }

        /// <summary>
        /// Load all of the WorkItemStatuses.
        /// (This is the master-list of statuses, not as they relate to individual WorkItems).
        /// Also loadsd WorkItemStatusFilter
        /// </summary>
        public void LoadWorkItemStatuses()
        {
            WorkItemStatuses = workItemRepository.GetWorkItemStatuses();

            // Load WorkItemStatusFilter
            foreach (WorkItemStatus wis in WorkItemStatuses)
            {
                WorkItemStatusFilter.Add(new WorkItemStatusFilter(wis.WorkItemStatusID, wis.StatusLabel));
            }
        }

        #region ApplicationMode
        private ApplicationMode _appMode = ApplicationMode.NOT_SET;
        public ApplicationMode AppMode
        {
            get { return _appMode; }
            set
            {
                _appMode = value;
                OnPropertyChanged("");   // Notify the UI
            }
        }

        /// <summary>
        /// Check to see if the application is in ADD_MODE
        /// </summary>
        public bool IsApplicationInAddMode
        {
            get
            {
                bool rValue = false;

                if (_appMode == ApplicationMode.ADD_MODE)
                    rValue = true;

                return rValue;
            }
        }

        public bool IsApplicationNotInAddMode
        {
            get
            {
                bool rValue = true;

                if (_appMode == ApplicationMode.ADD_MODE)
                    rValue = false;

                return rValue;
            }
        }
        #endregion

        public /*async*/ void AddWorkItem(WorkItem workItem)
        {
            /*await*/
            workItemRepository.InsertWorkItem(workItem);
            WorkItems.Add(workItem);
        }

/*        private WorkItemSaveCommand _saveCommand = new WorkItemSaveCommand();
        public WorkItemSaveCommand SaveCommand
        {
            get => _saveCommand;
            set
            {
                if (_saveCommand == null)
                    return;

                _saveCommand = value;
                OnPropertyChanged("");
            }
        }*/

        #region WorkItemCreatingCommand
        RelayCommand _workItemCreatingCommand;
        public ICommand WorkitemCreatingCommand
        {
            get
            {
                if (_workItemCreatingCommand == null)
                {
                    _workItemCreatingCommand = new RelayCommand(BeginWorkItemCreation, CanAddNewWorkItem);
                }
                return _workItemCreatingCommand;
            }
        }

        public void BeginWorkItemCreation()
        {
            AppMode = ApplicationMode.ADD_MODE;

            var wi = new WorkItem();
            var widd = new WorkItemDueDate(DateTime.Now.Date, "Initial WorkItem creation.");
            SelectedWorkItem = wi;
            //SelectedWorkItem.CurrentWorkItemDueDate = widd;
            //SelectedWorkItem.CurrentWorkItemDueDate = widd;
            ReceivedWorkItemDueDateChange(widd);
        }

        public bool CanAddNewWorkItem()
        {
            return true;
        }
        #endregion

        #region ShowDueDateDialogCommand
        RelayCommand _dueDateDialogCommand;
        public ICommand ShowDueDateDialogCommand
        {
            get
            {
                if (_dueDateDialogCommand == null)
                {
                    _dueDateDialogCommand = new RelayCommand(ShowDueDateDialog, CanShowDueDateDialog);
                }
                return _workItemCreatingCommand;
            }
        }

        public void ShowDueDateDialog()
        {

        }

        public bool CanShowDueDateDialog()
        {
            return true;
        }
        #endregion

        #region Cancel_WorkItem_CreatingCommand
        RelayCommand _workItemCancelCreationCommand;
        public ICommand CancelWorkItemCreatingCommand
        {
            get
            {
                if (_workItemCancelCreationCommand == null)
                {
                    _workItemCancelCreationCommand = new RelayCommand(CancelWorkItemCreation, CanCancelNewWorkItem);
                }
                return _workItemCancelCreationCommand;
            }
        }

        public void CancelWorkItemCreation()
        {
            Console.WriteLine("CancelWorkItemCreation in WorkItemViewModel");
            AppMode = ApplicationMode.EDIT_MODE;
        }

        public bool CanCancelNewWorkItem()
        {
            bool rValue;
            if (AppMode == ApplicationMode.ADD_MODE)
                rValue = true;
            else
                rValue = false;
            return rValue;
        }
        #endregion

        /// <summary>
        /// Delete a WorkItem
        /// </summary>
        public void DeleteWorkItem(WorkItem workItem)
        {
            if ((workItem == null) || (workItem.WorkItemID == null))
                return;

            Console.WriteLine("deletion");
            int workItemID = workItem.WorkItemID.Value;
            bool logicalDelete = GetAppPreferenceValueAsBool(PreferenceName.LOGICAL_DELETE);

            workItemRepository.DeleteWorkItemDueDate(workItemID, logicalDelete);
            workItemRepository.DeleteWorkItemStatusEntry(workItemID, logicalDelete);
            workItemRepository.DeleteWorkItem(workItemID, logicalDelete);
            WorkItems.Remove(_selectedWorkItem);
            _workItemOverview.View.Refresh();
        }

        RelayCommand _workItemSaveNewCommand;
        public ICommand WorkItemSaveNewCommand
        {
            get
            {
                if (_workItemSaveNewCommand == null)
                {
                    _workItemSaveNewCommand = new RelayCommand(SaveNewWorkItem, WorkItemReadyForSave);
                }
                return _workItemSaveNewCommand;
            }
        }

        /// <summary>
        /// Save a new WorkItem to the database and add it to the WorkItem collection.
        /// </summary>
        public void SaveNewWorkItem()
        {
            int workItemID = workItemRepository.InsertWorkItem(_selectedWorkItem);
            // Get default active status
            WorkItemStatus wis = GetWorkItemStatuses(true, true).ToList()[0];
            var wise = new WorkItemStatusEntry(workItemID, wis.WorkItemStatusID, 0, wis.StatusLabel);
            workItemRepository.InsertWorkItemStatusEntry(wise);
            _selectedWorkItem.CurrentWorkItemStatusEntry = wise;
            //            int daysToComplete = GetAppPreferenceValueAsInt(PreferenceName.DEFAULT_WORKITEM_LENGTH_DAYS);
            //            workItemRepository.InsertWorkItemDueDate(new WorkItemDueDate(workItemID, DateTime.Now.AddDays(daysToComplete), "Initial work item creation."));

//            WorkItemDueDate dueDate = _selectedWorkItem.CurrentWorkItemDueDate;
            if (_selectedWorkItem.CurrentWorkItemDueDate.WorkItemDueDateID < 1)
            {
                _selectedWorkItem.CurrentWorkItemDueDate.WorkItemID = workItemID;
                workItemRepository.InsertWorkItemDueDate(_selectedWorkItem.CurrentWorkItemDueDate);
            }

            WorkItems.Add(_selectedWorkItem);
            _workItemOverview.View.Refresh();

            AppMode = ApplicationMode.EDIT_MODE;
        }

        public bool CanDueDateBeChanged
        {
            get
            {
                if (SelectedWorkItemStatus == null)
                    return true;
                else 
                    return SelectedWorkItemStatus.IsConsideredActive;
            }

        }

        public bool WorkItemReadyForSave()
        {
            return true;
        }

        /// <summary>
        /// Return WorkItemStatus(es) based on their IsConsideredActive or IsDefault value.
        /// </summary>
        /// <param name="isConsideredActive"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        public IEnumerable<WorkItemStatus> GetWorkItemStatuses(bool? isConsideredActive = null, bool? isDefault = null)
        {
            IEnumerable<WorkItemStatus> rValue;

            // Return based on isActive
            if ((isConsideredActive.HasValue == true) && (isDefault.HasValue == false))
                rValue = WorkItemStatuses.Where(u => u.IsConsideredActive == isConsideredActive);
            // Return based on IsDefault (2 return items possible)
            else if ((isConsideredActive.HasValue == false) && (isDefault.HasValue == true))
                rValue = WorkItemStatuses.Where(u => u.IsDefault == isDefault);
            // Return based on isActive and IsDefault
            else if ((isConsideredActive.HasValue == true) && (isDefault.HasValue == true))
                rValue = WorkItemStatuses.Where(u => u.IsConsideredActive == isConsideredActive && u.IsDefault == isDefault);
            // Return all
            else
                rValue = WorkItemStatuses;

            return rValue;
        }

        private CollectionViewSource _workItemOverview;
        /// <summary>
        /// This CollectionViewSource provides a way in which the WorkItems Collection can be filtered.
        /// </summary>
        public ICollectionView WorkItemOverview
        {
            get
            {
                return _workItemOverview.View;
            }
        }

        private bool _workItemOverviewIsActiveFilter = true;
        /// <summary>
        /// Is the Overview panel (Work Item list) currently filtered to display only active statuses?
        /// </summary>
        public bool WorkItemOverviewIsActiveFilter
        {
            get => _workItemOverviewIsActiveFilter;
            set
            {
                _workItemOverviewIsActiveFilter = value;
                _workItemOverview.View.Refresh();
                OnPropertyChanged("");
            }
        }

        private bool _workItemOverviewIsClosedFilter = true;
        /// <summary>
        /// Is the Overview panel (Work Item list) currently filtered to display only closed statuses?
        /// </summary>
        public bool WorkItemOverviewIsClosedFilter
        {
            get => _workItemOverviewIsClosedFilter;
            set
            {
                _workItemOverviewIsClosedFilter = value;
                _workItemOverview.View.Refresh();
                OnPropertyChanged("");
            }
        }

        private string _workItemOverviewFilterText;
        /// <summary>
        /// Is the Overview panel (Work Item list) currently filtered to display only Work Items with the following text in title or description?
        /// </summary>
        public string WorkItemOverviewFilterText
        {
            get => _workItemOverviewFilterText;
            set
            {
                _workItemOverviewFilterText = value;
                _workItemOverview.View.Refresh();
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// Filter the WorkItems that are currently being displayed in the overview list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkItemOverviewFilter(object sender, FilterEventArgs e)
        {
            // A true value on e.Accepted results in the item being DISPLAYED.

            // Get each filter match
            WorkItem workItem = e.Item as WorkItem;
            bool textMatch = string.IsNullOrEmpty(WorkItemOverviewFilterText)
                || workItem.Title.Contains(WorkItemOverviewFilterText, StringComparison.OrdinalIgnoreCase)
                || workItem.Description.Contains(WorkItemOverviewFilterText, StringComparison.OrdinalIgnoreCase);
            WorkItemStatus wis = GetWorkItemStatus(workItem.CurrentWorkItemStatusEntry.WorkItemStatusID);
            bool isActiveMatch = _workItemOverviewIsActiveFilter && wis.IsConsideredActive;
            bool isClosedMatch = _workItemOverviewIsClosedFilter && wis.IsConsideredActive == false;
            //            bool isStatusFilter = 

            if (textMatch && (isActiveMatch || isClosedMatch))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

    }
}