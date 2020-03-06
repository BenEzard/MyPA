using MyPA.Code.Data.Actions;
using MyPA.Code.Data.Models;
using MyPA.Code.Data.Services;
using MyPA.Code.UI.Util;
using System.Windows.Input;

namespace MyPA.Code
{
    public class ApplicationViewModel : BaseViewModel
    {
        private IApplicationRepository applicationRepository = ApplicationRepository.Instance;

        private WorkItem _selectedWorkItem = null;
        public WorkItem SelectedWorkItem
        {
            get => _selectedWorkItem;
            set
            {
                _selectedWorkItem = value;
                OnPropertyChanged("");
            }
        }

        public bool IsWorkItemSelected
        {
            get
            {
                return _selectedWorkItem != null ? true : false;
            }
        }

        public ApplicationViewModel()
        {
            // Load Preferences
            Preferences = applicationRepository.GetApplicationPreferences();
            Messenger.Default.Register<WorkItemSelectedNotification>(this, OnWorkItemSelectedNotification);
        }

        public void OnWorkItemSelectedNotification(WorkItemSelectedNotification notification)
        {
            SelectedWorkItem = notification.WorkItem;
        }

        public string ApplicationNameAndVersion
        {
            get
            {
                return GetAppPreferenceValue(PreferenceName.APPLICATION_NAME) + " [v" + GetAppPreferenceValue(PreferenceName.APPLICATION_VERSION) + "]";
            }
        }

        public int ApplicationWidth
        {
            get
            {
                return GetAppPreferenceValueAsInt(PreferenceName.APPLICATION_WIDTH);
            }
        }

        public int ApplicationHeight
        {
            get
            {
                return GetAppPreferenceValueAsInt(PreferenceName.APPLICATION_HEIGHT);
            }
        }

        public int ApplicationPositionTop
        {
            get
            {
                return GetAppPreferenceValueAsInt(PreferenceName.APPLICATION_POSITION_TOP);
            }
        }

        public int ApplicationPositionLeft
        {
            get
            {
                return GetAppPreferenceValueAsInt(PreferenceName.APPLICATION_POSITION_LEFT);
            }
        }

        #region WorkItemCreatingCommand
        /// <summary>
        /// The WorkItemCreatingCommand sends out a message notifying listeners to the fact that a new WorkItem is being created.
        /// </summary>
        RelayCommand _workItemCreatingCommand;
        public ICommand WorkItemCreatingCommand
        {
            get
            {
                if (_workItemCreatingCommand == null)
                {
                    _workItemCreatingCommand = new RelayCommand(
                        /// Send out a notification that a WorkItem has begun creation.
                        () => { Messenger.Default.Send(new WorkItemCreatingNotification()); },
                        /// Can a New WorkItem be created at the moment?
                        () => { return true; }
                    );
                }
                return _workItemCreatingCommand;
            }
        }
        #endregion

        #region WorkItemDeletingCommand
        RelayCommand _workItemDeletingCommand;
        public ICommand WorkItemDeletingCommand
        {
            get
            {
                bool logical = GetAppPreferenceValueAsBool(PreferenceName.LOGICAL_DELETE);

                if (_workItemDeletingCommand == null)
                {
                    _workItemDeletingCommand = new RelayCommand(
                        // Send out a 'Work Item Deleting' notification.
                        () => { Messenger.Default.Send(new WorkItemDeletingNotification(_selectedWorkItem, logical)); }, 
                        // Button availability is controlled by IsEnabled binding instead of here.
                        null);
                }
                return _workItemDeletingCommand;
            }
        }
        #endregion

        #region WorkItemJournalCreatingCommand
        /// <summary>
        /// Send out a notification that a WorkItemJournal should begin creation.
        /// </summary>
        RelayCommand _workItemJournalCreatingCommand;
        public ICommand WorkItemJournalCreatingCommand
        {
            get
            {
                if (_workItemJournalCreatingCommand == null)
                {
                    _workItemJournalCreatingCommand = new RelayCommand(
                        // Send notification that a WorkItemJournal should begin creation
                        () => { Messenger.Default.Send(new WorkItemJournalCreatingNotification()); }, 
                        // Can a New WorkItem Journal be added now?
                        () => { return true; });
                }
                return _workItemJournalCreatingCommand;
            }
        }
        #endregion

        /// <summary>
        /// Save the current window location.
        /// TODO Change this to ICommand.
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="top"></param>
        /// <param name="left"></param>
        public void SaveWindowLocation(double height, double width, double top, double left)
        {
            // TODO Should these updates (also) be done in memory? Do we need to broadcast the change?
            applicationRepository.UpdatePreference(PreferenceName.APPLICATION_HEIGHT, height.ToString());
            applicationRepository.UpdatePreference(PreferenceName.APPLICATION_WIDTH, width.ToString());
            applicationRepository.UpdatePreference(PreferenceName.APPLICATION_POSITION_TOP, top.ToString());
            applicationRepository.UpdatePreference(PreferenceName.APPLICATION_POSITION_LEFT, left.ToString());
        }

        /// <summary>
        /// Send out a notification that the application is closing.
        /// </summary>
        public void SendApplicationClosingNotification()
        {
            if (GetAppPreferenceValueAsBool(PreferenceName.SAVE_SESSION_ON_EXIT))
                Messenger.Default.Send(new SaveSessionNotification());

            Messenger.Default.Send(new ApplicationClosingNotification());
        }
    }
}
