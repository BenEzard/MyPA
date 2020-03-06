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

namespace MyPA.Code
{
    public class WorkItemViewModel : BaseViewModel
    {
        private const string TAB_NAME = "TabTaskDescription";

        private IApplicationRepository applicationRepository = ApplicationRepository.Instance;

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

                // Do any required updates on the previously selected WorkItem, before changing the selection.
                UpdateWorkItem(_selectedWorkItem);

                _selectedWorkItem = value;

                Messenger.Default.Send(new WorkItemSelectedNotification(_selectedWorkItem));
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// Update the WorkItem.
        /// TODO: Note that this is always-fired, whether or not it is required.
        /// </summary>
        /// <param name="workItem"></param>
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

        /// <summary>
        /// Select a WorkItem by the workItemID.
        /// </summary>
        /// <param name="workItemID"></param>
        public void SelectWorkItem(int workItemID)
        {
            foreach (WorkItem wi in WorkItems)
            {
                if (wi.WorkItemID.Value == workItemID)
                {
                    SelectedWorkItem = wi;
                }
            }
        }

        /// <summary>
        /// The amount of completion of the selected WorkItem.
        /// This is bound to the slider control.
        /// </summary>
        public int SelectedWorkItemCompletion
        {
            get
            {
                int rValue = 0;

                if (_selectedWorkItem != null)
                {
                    if ((IsSelectedWorkItemSaved == false) || (_selectedWorkItem.WorkItemStatusEntryCount == 0))
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

        public WorkItemStatus SelectedWorkItemStatus
        {
            get
            {
                WorkItemStatus rValue = null;
                
                if (_selectedWorkItem != null)
                {
                    if ((IsSelectedWorkItemSaved == false) || (_selectedWorkItem.WorkItemStatusEntryCount == 0))
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

            RegisterListeners();

            LoadWorkItemStatuses();

            LoadWorkItems();

            if (GetAppPreferenceValueAsBool(PreferenceName.SAVE_SESSION_ON_EXIT))
                RestoreSession();
            else if (WorkItems.Count > 1)
                SelectedWorkItem = WorkItems[1];

            // Setup the WorkItemView.
            _workItemOverview = new CollectionViewSource();
            _workItemOverview.Source = WorkItems;
            _workItemOverview.Filter += WorkItemOverviewFilter;            

            OnPropertyChanged("");
        }

        /// <summary>
        /// Register all of the listeners for Messenger inputs.
        /// </summary>
        private void RegisterListeners()
        {
            Messenger.Default.Register<WorkItemCreatingNotification>(this, OnWorkItemCreatingNotification);
            
            Messenger.Default.Register<WorkItemDeletingNotification>(this, 
                (WorkItemDeletingNotification notification) => { OnWorkItemDeletingNotification(notification); });

            Messenger.Default.Register<ApplicationClosingNotification>(this, OnSaveApplicationClosing);
            Messenger.Default.Register<SaveSessionNotification>(this, OnSaveSessionNotification);
            Messenger.Default.Register<WorkItemDueDate>(this, ReceivedWorkItemDueDateChange);
            Messenger.Default.Register<WorkItemSelectTabAction>(this,
                (WorkItemSelectTabAction action) =>
                SelectedWorkItemTabIndex = GetUITabIndex(action.Name));

        }

        private void OnSaveApplicationClosing(ApplicationClosingNotification notification)
        {
            UpdateWorkItem(_selectedWorkItem);
        }

        /// <summary>
        /// Request to save any session preferences.
        /// When a 'SaveSessionNotification' is received this method is called.
        /// </summary>
        /// <param name="notification"></param>
        private void OnSaveSessionNotification(SaveSessionNotification notification)
        {
            applicationRepository.UpdatePreference(PreferenceName.LAST_SELECTED_WORK_ITEM, _selectedWorkItem.WorkItemID.ToString());
            applicationRepository.UpdatePreference(PreferenceName.LAST_SELECTED_WORK_ITEM_TAB, _selectedWorkItemTabIndex.ToString());
        }

        /// <summary>
        /// Restore the previous Session.
        /// </summary>
        private void RestoreSession()
        {
            SelectWorkItem(GetAppPreferenceValueAsInt(PreferenceName.LAST_SELECTED_WORK_ITEM));
            SelectedWorkItemTabIndex = GetAppPreferenceValueAsInt(PreferenceName.LAST_SELECTED_WORK_ITEM_TAB);
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

        #region WorkItemTabMethods
        /// <summary>
        /// List of tabs that are available on the UI.
        /// </summary>
        private List<string> _listOfUITabs = new List<string>();

        /// <summary>
        /// Register the NAME attribute of a tab.
        /// </summary>
        /// <param name="tabName"></param>
        public void RegisterUITab(string tabName)
        {
            _listOfUITabs.Add(tabName.ToUpper());
        }

        /// <summary>
        /// Records the int value of the currently selected WorkItem tab.
        /// </summary>
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
        #endregion

        /// <summary>
        /// Begin the creation of a new Work Item. 
        /// (Called on receipt of a WorkItemCreatingAction).
        /// (Note that the WorkItem is not saved at this time).
        /// </summary>
        private void OnWorkItemCreatingNotification(WorkItemCreatingNotification workItemCreatingAction)
        {
            // Change the WorkItem tab that is currently being displayed.
            SelectedWorkItemTabIndex = GetUITabIndex(TAB_NAME);

            SelectedWorkItem = new WorkItem(-1, DateTime.Now);
            DateTime newDueDate = new DueDateViewModel().GenerateDefaultDueDate();
            var widd = new WorkItemDueDate(newDueDate, "Initial WorkItem creation.");
            SelectedWorkItem.CurrentWorkItemDueDate = widd; // Note: Not saved yet because no WorkItemID

            InvokeEvent(this, workItemCreatingAction);
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
        /// Also loads WorkItemStatusFilter
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

        public bool IsSelectedWorkItemSaved
        {
            get
            {
                bool rValue = false;

                if ((_selectedWorkItem != null) && (_selectedWorkItem.WorkItemID.HasValue)) {
                    if (_selectedWorkItem.WorkItemID.Value == -1)
                        rValue = false;
                    else
                        rValue = true;
                }   
                    
                return rValue;
            }
        }

        public /*async*/ void AddWorkItem(WorkItem workItem)
        {
            /*await*/
            workItemRepository.InsertWorkItem(workItem);
            WorkItems.Add(workItem);
        }

        #region Cancel_WorkItem_CreatingCommand
        RelayCommand _workItemCancelCreationCommand;
        public ICommand CancelWorkItemCreatingCommand
        {
            get
            {
                if (_workItemCancelCreationCommand == null)
                {
                    _workItemCancelCreationCommand = new RelayCommand(
                        // Cancel the Work Item Creation
                        () => {
                            if (WorkItems.Count > 0)
                                SelectedWorkItem = WorkItems[0];
                        }, 
                        // Determine if the Cancel button should be available.
                        () => {
                            bool rValue;
                            if (IsSelectedWorkItemSaved)
                                rValue = false;
                            else
                                rValue = true;
                            return rValue;
                        });
                }
                return _workItemCancelCreationCommand;
            }
        }
        #endregion

        /// <summary>
        /// Delete a WorkItem and all associated data.
        /// </summary>
        /// <param name="notification"></param>
        private void OnWorkItemDeletingNotification(WorkItemDeletingNotification notification)
        {
            if ((notification == null) || (notification.WorkItem.WorkItemID == null))
                return;

            int workItemID = notification.WorkItem.WorkItemID.Value;
            workItemRepository.DeleteWorkItemDueDate(workItemID, notification.LogicalDelete);
            workItemRepository.DeleteWorkItemStatusEntry(workItemID, notification.LogicalDelete);
            workItemRepository.DeleteWorkItem(workItemID, notification.LogicalDelete);
            WorkItems.Remove(notification.WorkItem);

            // If there are no more work items, then notify.
            if (WorkItems.Count == 0)
            {
                SelectedWorkItem = null;
                Messenger.Default.Send(new WorkItemSelectedNotification(null));
            }

            _workItemOverview.View.Refresh();
        }

        RelayCommand _workItemSaveNewCommand;
        public ICommand WorkItemSaveNewCommand
        {
            get
            {
                if (_workItemSaveNewCommand == null)
                {
                    _workItemSaveNewCommand = new RelayCommand(SaveNewWorkItem, null);
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
            OnPropertyChanged("");
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

                if (_workItemOverview.View.Cast<object>().Count() == 0)
                    Messenger.Default.Send(new WorkItemSelectedNotification(null));
                
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

                if (_workItemOverview.View.Cast<object>().Count() == 0)
                    Messenger.Default.Send(new WorkItemSelectedNotification(null));

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

                if (_workItemOverview.View.Cast<object>().Count() == 0)
                    Messenger.Default.Send(new WorkItemSelectedNotification(null));

                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// Filter the WorkItems that are currently being displayed in the overview list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void WorkItemOverviewFilter(object sender, FilterEventArgs eventArgs)
        {
            // A true value on e.Accepted results in the item being DISPLAYED.

            // Get each filter match
            WorkItem workItem = eventArgs.Item as WorkItem;
            bool textMatch = string.IsNullOrEmpty(WorkItemOverviewFilterText)
                || workItem.Title.Contains(WorkItemOverviewFilterText, StringComparison.OrdinalIgnoreCase)
                || workItem.Description.Contains(WorkItemOverviewFilterText, StringComparison.OrdinalIgnoreCase);
            WorkItemStatus wis = GetWorkItemStatus(workItem.CurrentWorkItemStatusEntry.WorkItemStatusID);
            bool isActiveMatch = _workItemOverviewIsActiveFilter && wis.IsConsideredActive;
            bool isClosedMatch = _workItemOverviewIsClosedFilter && wis.IsConsideredActive == false;
            //            bool isStatusFilter = 

            if (textMatch && (isActiveMatch || isClosedMatch))
            {
                eventArgs.Accepted = true;
            }
            else
            {
                eventArgs.Accepted = false;
            }
        }

    }
}