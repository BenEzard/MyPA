using MyPA.Code.Data;
using MyPA.Code.Data.Actions;
using MyPA.Code.Data.Events;
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

        public delegate void ModelEventHandler(object obj, WorkItemJournalEvent e);
        public event ModelEventHandler ModelEvent;

        private ApplicationMode _appMode = ApplicationMode.EDIT_MODE;
        public ApplicationMode AppMode
        {
            get => _appMode;
            set {
                _appMode = value;
                OnPropertyChanged("");
            }
        }

        public bool IsApplicationInAddMode 
        {
            get
            {
                if (_appMode == ApplicationMode.ADD_MODE)
                    return true;
                else
                    return false;
            }
        }

        public bool IsApplicationNotInAddMode
        {
            get
            {
                if (_appMode == ApplicationMode.ADD_MODE)
                    return false;
                else
                    return true;
            }
        }

        public WorkItemJournalViewModel()
        {
            Messenger.Default.Register<WorkItemSelectedAction>(this, WorkItemSelected);
            Messenger.Default.Register<WorkItemJournalCreatingAction>(this, WorkItemJournalCreatingNotification);
        }

        public void UpdateJournalEntry()
        {
            Console.WriteLine($"===> inside UpdateJournalEntry {_selectedJournal.WorkItemJournalID}");
            if (_selectedJournal.WorkItemJournalID < 1)
            {
                journalRepository.InsertWorkItemJournalEntry(_selectedJournal);
                WorkItemJournal.Add(_selectedJournal);
            }
            else
            {
                journalRepository.UpdateWorkItemJournalEntry(_selectedJournal);
            }
        }

        public void WorkItemSelected(WorkItemSelectedAction action)
        {
            if ((action.WorkItem == null) || (action.WorkItem.WorkItemID.HasValue == false))
                return;

            _workItemID = action.WorkItem.WorkItemID.Value;
            WorkItemJournal = new ObservableCollection<WorkItemJournalEntry>(journalRepository.SelectWorkItemJournals(_workItemID));
        }

        public void WorkItemJournalCreatingNotification(WorkItemJournalCreatingAction action)
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
                    _moveVerticalSplitEqualCommand = new RelayCommand(MoveVerticalSplitEqual, null);
                }
                return _moveVerticalSplitEqualCommand;
            }
        }

        /// <summary>
        /// Delete a WorkItem
        /// </summary>
        public void MoveVerticalSplitEqual()
        {
            ModelEvent?.Invoke(this, new WorkItemJournalEvent(WorkItemJournalAction.MOVE_VERTICAL_SPLIT, SplitSetting.EQUAL_SPLIT));
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
                    _moveVerticalSplitEqualLeftCommand = new RelayCommand(MoveVerticalSplitLeft, null);
                }
                return _moveVerticalSplitEqualLeftCommand;
            }
        }

        /// <summary>
        /// Delete a WorkItem
        /// </summary>
        public void MoveVerticalSplitLeft()
        {
            ModelEvent?.Invoke(this, new WorkItemJournalEvent(WorkItemJournalAction.MOVE_VERTICAL_SPLIT, SplitSetting.LEFT_EXPANDED));
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
                    _moveVerticalSplitEqualRightCommand = new RelayCommand(MoveVerticalSplitRight, null);
                }
                return _moveVerticalSplitEqualRightCommand;
            }
        }

        /// <summary>
        /// Delete a WorkItem
        /// </summary>
        public void MoveVerticalSplitRight()
        {
            ModelEvent?.Invoke(this, new WorkItemJournalEvent(WorkItemJournalAction.MOVE_VERTICAL_SPLIT, SplitSetting.RIGHT_EXPANDED));
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
        public void WorkItemJournalCreating()
        {
            // If the application is not yet in add mode, prep it.
            if (IsApplicationNotInAddMode)
            {
                // Notify the UI that a new WorkItemJournal is being created.
                // This changes the visible tab and sets the vertical split, and the focus to the Journal Title textbox.
                ModelEvent?.Invoke(this, new WorkItemJournalEvent(WorkItemJournalAction.CREATING_WORK_ITEM_JOURNAL_ENTRY));

                SelectedJournal = new WorkItemJournalEntry(_workItemID);

                AppMode = ApplicationMode.ADD_MODE;
            }

            else
            { // Otherwise, respond to the button click.
//                journalRepository.InsertWorkItemJournalEntry(_selectedJournal);
//                WorkItemJournal.Add(_selectedJournal);
                
                AppMode = ApplicationMode.EDIT_MODE;
            }

        }
        #endregion

        #region WorkItemJournalEditingCommand
        RelayCommand _workItemJournalEditingCommand;
        public ICommand WorkItemJournalEditingCommand
        {
            get
            {
                if (_workItemJournalEditingCommand == null)
                {
                    _workItemJournalEditingCommand = new RelayCommand(
                        () =>
                        {
                            // Notify the UI that a new WorkItemJournal is being created.
                            // This changes the visible tab and sets the vertical split, and the focus to the Journal Title textbox.
                            ModelEvent?.Invoke(this, new WorkItemJournalEvent(WorkItemJournalAction.EDITING_WORK_ITEM_JOURNAL_ENTRY));
                        }, 
                        null);
                }
                return _workItemJournalEditingCommand;
            }
        }
        #endregion

    }



}
