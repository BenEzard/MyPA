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

        private WorkItemStatus GetWorkItemStatus(int id)
        {
            WorkItemStatus rValue = null;
            foreach (WorkItemStatus wis in WorkItemStatuses)
            {
                if (wis.WorkItemStatusID == id)
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
            get { return _selectedWorkItem; }
            set
            {
                _selectedWorkItem = value;
                _selectedWorkItemStatus = GetWorkItemStatus(_selectedWorkItem.CurrentWorkItemStatusEntry.WorkItemStatusID);
                OnPropertyChanged("");
                Console.WriteLine($"Selection made {_selectedWorkItem.Title}, has a WorkItemStatus of {_selectedWorkItem.CurrentWorkItemStatusEntry.WorkItemStatusID}");
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

        private int _selectedWorkItemCompletion;
        public int SelectedWorkItemCompletion
        {
            get
            {
                return _selectedWorkItemCompletion;
            }
            set
            {
                _selectedWorkItemCompletion = value;
                var wise = new WorkItemStatusEntry(_selectedWorkItem.WorkItemID.Value, _selectedWorkItemStatus.WorkItemStatusID, _selectedWorkItemCompletion);
                workItemRepository.InsertWorkItemStatusEntry(wise);
                _selectedWorkItem.CurrentWorkItemStatusEntry = wise;
                OnPropertyChanged("");
                Console.WriteLine($"Completion amount changed {value}");
            }
        }

        private WorkItemStatus _selectedWorkItemStatus;
        public WorkItemStatus SelectedWorkItemStatus
        {
            get
            {
                return _selectedWorkItemStatus;
            }
            set
            {
                ChangeWorkItemStatus(_selectedWorkItemStatus, value);
                OnPropertyChanged("");
            }
        }

        public WorkItemViewModel()
        {
            ApplicationViewModel appViewModel = new ApplicationViewModel();

            // Load Preferences
            Preferences = workItemRepository.GetWorkItemPreferences();

            LoadWorkItems();
            LoadWorkItemStatuses();
            Console.WriteLine($"number of list items = {ActiveWorkItems.Count}");
            OnPropertyChanged("");
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
            // If the old value is a Closed status and the new value is an opened status, then set completion amount to X
            if ((oldValue.IsConsideredActive == false) && (newValue.IsConsideredActive == true))
            {
                _selectedWorkItemCompletion = GetAppPreferenceValueAsInt(PreferenceName.STATUS_COMPLETE_TO_ACTIVE);
            }

            // If it is changed to the default closed status, then set completion to 100%
            if ((newValue.IsConsideredActive == false) && (newValue.IsDefault == true))
            {
                _selectedWorkItemCompletion = 100;
            }

            var wise = new WorkItemStatusEntry(_selectedWorkItem.WorkItemID.Value, newValue.WorkItemStatusID, SelectedWorkItemCompletion);
            workItemRepository.InsertWorkItemStatusEntry(wise);
            _selectedWorkItem.CurrentWorkItemStatusEntry = wise;
            _selectedWorkItemStatus = newValue;

        }

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
            workItemRepository.AddWorkItem(workItem);
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
            Console.WriteLine("inside BeginWorkItemCreation (2)");
            //            WorkItemCreatingEvent?.Invoke(this, new WorkItemEventArgs(WorkItemType.WORK_ITEM_CREATING, wi));
            Console.WriteLine("inside BeginWorkItemCreation (3)");
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
    }
}