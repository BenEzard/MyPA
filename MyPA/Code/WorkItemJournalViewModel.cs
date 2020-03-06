using MyPA.Code.Data.Actions;
using MyPA.Code.Data.Services;
using MyPA.Code.UI;
using MyPA.Code.UI.Util;
using System;
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
            
        }

        public WorkItemJournalViewModel()
        {
            Preferences = journalRepository.GetWorkItemJournalPreferences();

            Messenger.Default.Register<WorkItemSelectedNotification>(this, OnWorkItemSelectedNotification);
            Messenger.Default.Register<WorkItemJournalCreatingNotification>(this, OnWorkItemJournalCreatingNotification);
            Messenger.Default.Register<WorkItemDeletingNotification>(this, OnWorkItemDeletingNotification);
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
        public void OnWorkItemSelectedNotification(WorkItemSelectedNotification notification)
        {
            Console.WriteLine($"WorkItem selection has been received in Journal");
            // If WorkItem has not been instantiated or saved yet, then don't try to do anything with the journals.
            if ((notification.WorkItem == null) || (notification.WorkItem.WorkItemID.HasValue == false) || (notification.WorkItem.WorkItemID.Value == -1))
                return;

            _workItemID = notification.WorkItem.WorkItemID.Value;
            WorkItemJournal = new ObservableCollection<WorkItemJournalEntry>(journalRepository.SelectWorkItemJournals(_workItemID));
            Console.WriteLine($"{WorkItemJournal.Count} journal entries loaded.");
            Messenger.Default.Send(new MoveVerticalWorkItemJournalSplitNotification(SplitSetting.EQUAL_SPLIT));
        }

        public void OnWorkItemJournalCreatingNotification(WorkItemJournalCreatingNotification notification)
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
        /// 
        /// </summary>
        /// <param name="notification"></param>
        public void WorkItemJournalCreating()
        {
            Console.WriteLine("in WorkItemJournalCreating");
            // If the application is not yet in add mode, prep it.
//            if (IsSelectedWorkItemJournalSaved == false)
//            {
                // Notify the UI that a new WorkItemJournal is being created.
                // This changes the visible tab and sets the vertical split, and the focus to the Journal Title textbox.
                InvokeEvent(this, new WorkItemJournalCreatingNotification());

                SelectedJournal = new WorkItemJournalEntry(_workItemID);
//            }

//            else
//            { // Otherwise, respond to the button click.
//                journalRepository.InsertWorkItemJournalEntry(_selectedJournal);
//                WorkItemJournal.Add(_selectedJournal);
                
//                AppMode = ApplicationMode.EDIT_MODE;
//            }

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
    }



}
