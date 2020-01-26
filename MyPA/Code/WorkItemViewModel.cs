using MyPA.Code.Data.Services;
using MyPA.Code.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MyPA.Code.UI.Util;
using System.Linq;

namespace MyPA.Code
{
    public class WorkItemViewModel : BaseViewModel
    {
        /// <summary>
        /// The Repository handles all of the data collection; editing and deleting methods.
        /// </summary>
        private IWorkItemRepository workItemRepository = new WorkItemRepository();

        /// <summary>
        /// An ObservableCollection of all 'active' WorkItems.
        /// (Potentially this will change to be all work items, if I can work out how to filter the data well).
        /// </summary>
        public ObservableCollection<WorkItem> ActiveWorkItems { get; set; } = new ObservableCollection<WorkItem>();

        /// <summary>
        /// A list of all WorkItemStatuses.
        /// This is to populate the UI, and is NOT related to any particular WorkItem.
        /// </summary>
        public List<WorkItemStatus> WorkItemStatuses { get; set; }

        private WorkItem _selectedWorkItem = null;


        private int _selectedWorkItemCompletion;
        private WorkItemStatus _selectedWorkItemStatus;


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


        /// <summary>
        /// The currently selected WorkItem.
        /// </summary>
        public WorkItem SelectedWorkItem
        {
            get { return _selectedWorkItem; }
            set
            {
                _selectedWorkItem = value;

/*                if (_selectedWorkItem != null) // If no WorkItem was selected before...
                {
                    // Check if save is required for the currently selected item before changing.
                    if (_selectedWorkItem.WorkItemID == -1)
                    {
                        Console.WriteLine("Attempting INSERT of WI");
                        workItemRepository.InsertWorkItem(_selectedWorkItem);
                        var list = GetWorkItemStatuses(true, true).ToArray(); // Method will only return one value when (true, true)
                        var wise = new WorkItemStatusEntry(_selectedWorkItem.WorkItemID.Value, list[0].WorkItemStatusID, 0, GetWorkItemStatus(list[0].WorkItemStatusID).StatusLabel);
                        workItemRepository.InsertWorkItemStatusEntry(wise);
                        _selectedWorkItem.CurrentWorkItemStatusEntry = wise;
                        ActiveWorkItems.Add(_selectedWorkItem);
                    }
                    else
                    {
                        Console.WriteLine("Attempting UPDATE of WI");
                        workItemRepository.UpdateWorkItem(_selectedWorkItem);
                    }
                }

                _selectedWorkItem = value;
                if (_selectedWorkItem.CurrentWorkItemStatusEntry == null)
                {
                    // Create a new default active version
                    var list = GetWorkItemStatuses(true, true).ToArray(); // Method will only return one value when (true, true)
                    _selectedWorkItem.CurrentWorkItemStatusEntry = new WorkItemStatusEntry(-1, list[0].WorkItemStatusID, 0, GetWorkItemStatus(list[0].WorkItemStatusID).StatusLabel);
                }
                _selectedWorkItemStatus = GetWorkItemStatus(_selectedWorkItem.CurrentWorkItemStatusEntry.WorkItemStatusID);*/
                
                OnPropertyChanged("");
                
                Console.WriteLine($"Selection made {_selectedWorkItem.Title}, has a WorkItemStatus of {_selectedWorkItem.CurrentWorkItemStatusEntry.WorkItemStatusID}");
            }
        }

        /// <summary>
        /// A VM variable mirroring SelectedWorkItem.Title; bound by the view.
        /// </summary>
        public string Title {
            get {
                string rValue = "";
                if (_selectedWorkItem != null)
                    rValue = _selectedWorkItem.Title;
                return rValue; 
            }
            set {
                _selectedWorkItem.Title = value;
                OnPropertyChanged("");
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

        

        /*        public void NotSure()
                {
                    Console.WriteLine("not sure");
                }*/

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

        public int SelectedWorkItemCompletion
        {
            get
            {
                int rValue = 0;

                if (_selectedWorkItem != null)
                {
                    rValue = _selectedWorkItem.GetLastWorkItemStatusEntry().CompletionAmount;
                    Console.WriteLine($"...The returned value is {rValue}");
                }

                return rValue;
            }
            set
            {
                // Create a new WorkItemStatusEntry to record the change in the completion amount.
                var wise = GenerateNewWorkItemStatusEntry(value);
                workItemRepository.InsertWorkItemStatusEntry(wise);
                _selectedWorkItem.LastWorkItemStatusEntry = wise;
                OnPropertyChanged("");
            }
        }

        public WorkItemStatus SelectedWorkItemStatus
        {
            get
            {
                WorkItemStatus rValue = null;
                
                if (_selectedWorkItem != null)
                    rValue = GetWorkItemStatus(_selectedWorkItem.GetLastWorkItemStatusEntry().WorkItemStatusID);
                
                return rValue;
            }
            set
            {
                ChangeWorkItemStatus(GetWorkItemStatus(_selectedWorkItem.GetLastWorkItemStatusEntry().WorkItemStatusID), value);
                OnPropertyChanged("");
            }
        }

        public WorkItemViewModel()
        {
            ApplicationViewModel appViewModel = new ApplicationViewModel();

            // Load Preferences
            Preferences = workItemRepository.GetWorkItemPreferences();

            Messenger.Default.Register<AppAction>(this, RequestAddNewWorkItem);

            LoadWorkItemStatuses();
            LoadWorkItems();
            Console.WriteLine($"number of list items = {ActiveWorkItems.Count}");
            OnPropertyChanged("");
        }

        private WorkItemStatusEntry GenerateNewWorkItemStatusEntry(int completionAmount)
        {
            var currentWIS = GetWorkItemStatus(_selectedWorkItem.GetLastWorkItemStatusEntry().WorkItemStatusID);
            var wise = new WorkItemStatusEntry(_selectedWorkItem.WorkItemID.Value, currentWIS.WorkItemStatusID, completionAmount, currentWIS.StatusLabel);
            return wise;
        }

        private void RequestAddNewWorkItem(AppAction action)
        {
            if (action == AppAction.CREATING_WORK_ITEM)
            {
                Console.WriteLine($"Creating WorkItem...>");
                AppMode = ApplicationMode.ADD_MODE;
                SelectedWorkItem = new WorkItem();
            }
        }

        /// <summary>
        /// Loads all of the WorkItems from the repository into the ActiveWorkItems collection.
        /// </summary>
        private void LoadWorkItems()
        {
            foreach (WorkItem wi in workItemRepository.GetWorkItems(GetAppPreferenceValueAsInt(PreferenceName.LOAD_STALE_DAYS)))
            {
                ActiveWorkItems.Add(wi);
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
            workItemRepository.InsertWorkItemStatusEntry(wise);
            _selectedWorkItem.LastWorkItemStatusEntry= wise;
        }

        /// <summary>
        /// Load all of the WorkItemStatuses.
        /// (This is the master-list of statuses, not as they relate to individual WorkItems).
        /// </summary>
        public void LoadWorkItemStatuses()
        {
            WorkItemStatuses = workItemRepository.GetWorkItemStatuses();
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
            ActiveWorkItems.Add(workItem);
        }

        /*        public async void SaveWorkItem()
                {
                    if (SelectedWorkItem.WorkItemId == null)
                        AddWorkItem(SelectedWorkItem);
                    else 
                        await workItemRepository.UpdateWorkItemAsync(SelectedWorkItem);
                }*/

        //        public event EventHandler<WorkItemEventArgs> WorkItemCreatingEvent;

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
            Console.WriteLine("inside BeginWorkItemCreation (1)");
            AppMode = ApplicationMode.ADD_MODE;

            var wi = new WorkItem();
            SelectedWorkItem = wi;
        }

        public bool CanAddNewWorkItem()
        {
            return true;
        }
        #endregion

        #region CancelWorkItemCreatingCommand
        RelayCommand _workItemCamcelCreatingCommand;
        public ICommand CancelWorkItemCreatingCommand
        {
            get
            {
                if (_workItemCamcelCreatingCommand == null)
                {
                    _workItemCamcelCreatingCommand = new RelayCommand(CancelWorkItemCreation, CanCancelNewWorkItem);
                }
                return _workItemCamcelCreatingCommand;
            }
        }

        public void CancelWorkItemCreation()
        {
            AppMode = ApplicationMode.EDIT_MODE;
        }

        public bool CanCancelNewWorkItem()
        {
            if (AppMode == ApplicationMode.ADD_MODE)
                return true;
            else
                return false;
        }
        #endregion

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

        public void SaveNewWorkItem()
        {
            Console.WriteLine("Saving");
            workItemRepository.InsertWorkItem(_selectedWorkItem);
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
    }
}