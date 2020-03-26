using MyPA.Code.Data.Actions;
using MyPA.Code.Data.Models;
using MyPA.Code.Data.Services;
using MyPA.Code.UI;
using MyPA.Code.UI.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyPA.Code
{
    public class WorkItemJournalViewModel : BaseViewModel
    {
        /// <summary>
        /// The Repository handles all of the data collection; editing and deleting methods.
        /// </summary>
        private IWorkItemJournalRepository journalRepository = new WorkItemJournalRepository();

        private ObservableCollection<WorkItemJournalEntry> _workItemJournal = new ObservableCollection<WorkItemJournalEntry>();
        public ObservableCollection<WorkItemJournalEntry> WorkItemJournal {
            get => _workItemJournal;
            set {
                _workItemJournal = value;
                OnPropertyChanged("");
            }
        }

        private int _workItemID = -1;

        private WorkItemJournalEntry _selectedJournal;
        public WorkItemJournalEntry SelectedJournal
        {
            get => _selectedJournal;
            set
            {
                _selectedJournal = value;
                OnPropertyChanged("");
            }
        }

        public bool IsJournalEntrySelected
        {
            get
            {
                if (_selectedJournal == null)
                    return false;
                else
                    return true;
            }
        }

        private SplitSetting _verticalSplit = SplitSetting.EQUAL_SPLIT;
        public SplitSetting VerticalSplit
        {
            get => _verticalSplit;
            set
            {
                _verticalSplit = value;
                OnPropertyChanged("");
            }
        }

/*        /// <summary>
        /// 
        /// </summary>
        public bool IsSelectedWorkItemJournalSaved
        {
            get
            {
                bool rValue = false;
                if (_selectedJournal != null)
                {
                    if (_selectedJournal.WorkItem_ID == -1)
                        rValue = false;
                    else
                        rValue = true;
                }
                return rValue;
            }   
        }*/

        public bool AddingWorkItemJournal
        {
            get
            {
                bool rValue = false;
                if (_selectedJournal == null)
                    Console.WriteLine($"AddingWorkItemJournal: _selectedJournal is null");
                else
                    Console.WriteLine($"AddingWorkItemJournal: {_selectedJournal.WorkItem_ID}");
                
                if (_selectedJournal == null) {
                    rValue = false;
                }
                else
                {
                    if (_selectedJournal.WorkItem_ID == -1)
                        rValue = true;
                }

                return rValue;
            }
        }

        public WorkItemJournalViewModel()
        {
            Preferences = journalRepository.GetWorkItemJournalPreferences();

            Messenger.Default.Register<WorkItemSelectedNotification>(this, OnWorkItemSelectedNotification);
            Messenger.Default.Register<WorkItemJournalCreatingNotification>(this, OnWorkItemJournalCreatingNotification);
            Messenger.Default.Register<WorkItemDeletingNotification>(this, OnWorkItemDeletingNotification);

            WorkItem wi = ApplicationViewModel.Instance.SelectedWorkItem;
            Console.WriteLine($"item from application is {wi.Title}");
            OnWorkItemSelectedNotification(new WorkItemSelectedNotification(wi));
            OnPropertyChanged("");
        }

        /// <summary>
        /// Method which is run when 'OnWorkItemDeleting' notification is received.
        /// </summary>
        /// <param name="notification"></param>
        private void OnWorkItemDeletingNotification(WorkItemDeletingNotification notification)
        {
            journalRepository.DeleteAllWorkItemJournalEntries(notification.WorkItem.WorkItemID.Value, notification.LogicalDelete);
        }

        /// <summary>
        /// Update the selected journal entry; either inserting or updating it.
        /// </summary>
        public void UpdateJournalEntry()
        {
            if (_selectedJournal == null || (String.IsNullOrEmpty(_selectedJournal.Title)) || (String.IsNullOrEmpty(_selectedJournal.Entry)))
                return;

            Console.WriteLine($"Inside UpdateJournalEntry {_selectedJournal.WorkItemJournalID}");
            if (_selectedJournal.WorkItemJournalID < 1)
            {
                Console.WriteLine("Inserting");
                journalRepository.InsertWorkItemJournalEntry(_selectedJournal);
                WorkItemJournal.Add(_selectedJournal);
            }
            else
            {
                Console.WriteLine("Updating");
                journalRepository.UpdateWorkItemJournalEntry(_selectedJournal);
            }
        }

        /// <summary>
        /// Method is called when a 'WorkItemSelectedNotification' is received.
        /// </summary>
        /// <param name="notification"></param>
        private void OnWorkItemSelectedNotification(WorkItemSelectedNotification notification)
        {
            Console.Write("in OnWorkItemSelectedNotification...");
            // If WorkItem has not been instantiated or saved yet, then don't try to do anything with the journals.
            if ((notification.WorkItem == null) || (notification.WorkItem.WorkItemID.HasValue == false) || (notification.WorkItem.WorkItemID.Value == -1))
            {
                Console.WriteLine("bailing, nothing is selected yet.");
                return;
            }

//            Console.WriteLine("in OnWorkItemSelectedNotification");
            _workItemID = notification.WorkItem.WorkItemID.Value;
            WorkItemJournal = new ObservableCollection<WorkItemJournalEntry>(journalRepository.SelectWorkItemJournals(_workItemID));
            Console.WriteLine($"{WorkItemJournal.Count} journal entries loaded.");
            Messenger.Default.Send(new MoveVerticalWorkItemJournalSplitNotification(SplitSetting.EQUAL_SPLIT));
        }

        /// <summary>
        /// This method is called when a WorkItemJournalCreatingNotification is received.
        /// </summary>
        /// <param name="notification"></param>
        private void OnWorkItemJournalCreatingNotification(WorkItemJournalCreatingNotification notification)
        {
            WorkItemJournalCreating(); // This calls another method, so that both buttons result in going to the same logic.
        }

        #region MoveJournalVerticalSplitEqualCommand
        RelayCommand _moveVerticalSplitEqualCommand;
        public ICommand MoveVerticalSplitEqualCommand
        {
            get
            {
                if (_moveVerticalSplitEqualCommand == null)
                {
                    _moveVerticalSplitEqualCommand = new RelayCommand(
                        // Send out an event that requests a change to the vertical split.
                        () => { InvokeEvent(this, new MoveVerticalWorkItemJournalSplitNotification(SplitSetting.EQUAL_SPLIT)); }
                        // Command is always available.
                        , null);
                }
                return _moveVerticalSplitEqualCommand;
            }
        }
        #endregion

        #region MoveJournalVerticalSplitLeftCommand
        RelayCommand _moveVerticalSplitEqualLeftCommand;
        public ICommand MoveVerticalSplitLeftCommand
        {
            get
            {
                if (_moveVerticalSplitEqualLeftCommand == null)
                {
                    _moveVerticalSplitEqualLeftCommand = new RelayCommand(
                        // Send out an event that requests a change to the vertical split.
                        () => { InvokeEvent(this, new MoveVerticalWorkItemJournalSplitNotification(SplitSetting.LEFT_EXPANDED)); }
                        // Command is always available.
                        , null);
                }
                return _moveVerticalSplitEqualLeftCommand;
            }
        }
        #endregion

        #region MoveJournalVerticalSplitRightCommand
        RelayCommand _moveVerticalSplitEqualRightCommand;
        public ICommand MoveVerticalSplitRightCommand
        {
            get
            {
                if (_moveVerticalSplitEqualRightCommand == null)
                {
                    _moveVerticalSplitEqualRightCommand = new RelayCommand(
                        // Send out an event that requests a change to the vertical split.
                        () => { InvokeEvent(this, new MoveVerticalWorkItemJournalSplitNotification(SplitSetting.RIGHT_EXPANDED)); }
                        // Command is always available.
                        , null);
                }
                return _moveVerticalSplitEqualRightCommand;
            }
        }
        #endregion


        #region WorkItemJournalCreatingCommand
        RelayCommand _workItemJournalCreatingCommand;
        public ICommand WorkItemJournalCreatingCommand
        {
            get
            {
                if (_workItemJournalCreatingCommand == null)
                {
                    _workItemJournalCreatingCommand = new RelayCommand(WorkItemJournalCreating, null);
                }
                return _workItemJournalCreatingCommand;
            }
        }

        /// <summary>
        /// Start preparing the view and creating a WorkItemJournal
        /// </summary>
        /// <param name="notification"></param>
        public void WorkItemJournalCreating()
        {
            // Select the 'Detail' Journal tab
            SelectedJournalTabIndex = GetJournalUITabIndex(JournalTabType.JOURNAL_DETAIL);

            // Notify the UI that a new WorkItemJournal is being created.
            // This changes the visible tab, sets the focus to the Journal Title textbox and (potentially) sets the vertical split.
            var notfication = new WorkItemJournalCreatingNotification();
            notfication.GiveDetailTabProminence = GetAppPreferenceValueAsBool(PreferenceName.JOURNAL_ON_CREATION_GIVE_PROMINENCE);
            InvokeEvent(this, notfication);
            
            SelectedJournal = new WorkItemJournalEntry(_workItemID);

        }
        #endregion

        #region WorkItemJournalEntryDeleteCommand
        RelayCommand _workItemJournalEntryDeletingCommand;
        public ICommand WorkItemJournalEntryDeleteCommand
        {
            get
            {
                bool logical = GetAppPreferenceValueAsBool(PreferenceName.LOGICAL_DELETE);
                if (_workItemJournalEntryDeletingCommand == null)
                {
                    _workItemJournalEntryDeletingCommand = new RelayCommand(
                        () => { 
                            journalRepository.DeleteWorkItemJournalEntry(_selectedJournal.WorkItemJournalID.Value, logical);
                            WorkItemJournal.Remove(_selectedJournal);
                        }
                        , null);
                }
                return _workItemJournalEntryDeletingCommand;
            }
        }
        #endregion

        #region SelectJournalDetailTabCommand
        RelayCommand _workItemJournalSelectDetailTabCommand;
        public ICommand WorkItemJournalSelectDetailTabCommand
        {
            get
            {
                if (_workItemJournalSelectDetailTabCommand == null)
                {
                    _workItemJournalSelectDetailTabCommand = new RelayCommand(
                        // What to do when the button is selected
                        () => {
                            SelectedJournalTabIndex = GetJournalUITabIndex(JournalTabType.JOURNAL_DETAIL);
                        },
                        // When the button is not available
                        null
                        );
                }
                return _workItemJournalSelectDetailTabCommand;
            }
        }
        #endregion

        #region SelectJournalOverviewTabCommand
        RelayCommand _workItemJournalOverviewTabCommand;
        public ICommand WorkItemJournalSelectOverviewTabCommand
        {
            get
            {
                if (_workItemJournalOverviewTabCommand == null)
                {
                    _workItemJournalOverviewTabCommand = new RelayCommand(
                        // What to do when the button is selected
                        () => {
                            SelectedJournalTabIndex = GetJournalUITabIndex(JournalTabType.JOURNAL_OVERVIEW);
                        },
                        // When the button is not available
                        null
                        );
                }
                return _workItemJournalOverviewTabCommand;
            }
        }
        #endregion

        #region CancelJournalAdditionCommand
        RelayCommand _cancelJournalAddCommand;
        public ICommand CancelWorkItemJournalAddCommand
        {
            get
            {
                if (_cancelJournalAddCommand == null)
                {
                    _cancelJournalAddCommand = new RelayCommand(
                        // What to do when the button is selected
                        () => {
                            // Return to the overview tab.
                            SelectedJournalTabIndex = GetJournalUITabIndex(JournalTabType.JOURNAL_OVERVIEW);

                            // Send out an event to get the vertical split to return to 50%/50%
                            InvokeEvent(this, new MoveVerticalWorkItemJournalSplitNotification(SplitSetting.EQUAL_SPLIT));

                            SelectedJournal = null;
                        },
                        // When the button is not available
                        null
                        );
                }
                return _cancelJournalAddCommand;
            }
        }
        #endregion

        #region JournalUITabs
        /// <summary>
        /// List of tabs that are available on the UI.
        /// </summary>
        private Dictionary<JournalTabType, UITab> _journalUITabs = new Dictionary<JournalTabType, UITab>();

        public void RegisterJournalUITab(JournalTabType tabType, string tabName)
        {
            int currentCounter = _journalUITabs.Count;
            _journalUITabs.Add(tabType, new UITab(tabName, currentCounter));
        }

        /// <summary>
        /// Records the int value of the currently selected Journal TabItem.
        /// </summary>
        private int _selectedJournalTabIndex;
        public int SelectedJournalTabIndex
        {
            get => _selectedJournalTabIndex;
            set
            {
                _selectedJournalTabIndex = value;
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// Return the tab index based on the TabItem Name.
        /// </summary>
        /// <param name="tabName"></param>
        /// <returns></returns>
        private int GetJournalUITabIndex(JournalTabType type)
        {
            if (_journalUITabs.Count == 0)
                return -1;

            _journalUITabs.TryGetValue(type, out UITab tab);
            return tab.TabIndex;
        }
        #endregion
    }



}
