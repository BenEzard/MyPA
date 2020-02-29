using MyPA.Code.Data.Actions;
using MyPA.Code.Data.Services;
using MyPA.Code.UI.Util;
using System.Windows.Input;

namespace MyPA.Code
{
    public class ApplicationViewModel : BaseViewModel
    {
        private IApplicationRepository applicationRepository = new ApplicationRepository();

        public ApplicationViewModel()
        {
            // Load Preferences
            Preferences = applicationRepository.GetApplicationPreferences();
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
        RelayCommand _workItemCreatingCommand;
        public ICommand WorkItemCreatingCommand
        {
            get
            {
                if (_workItemCreatingCommand == null)
                {
                    _workItemCreatingCommand = new RelayCommand(
                        /// Send out a notification that a WorkItem has begun creation.
                        () => { Messenger.Default.Send(new WorkItemCreatingAction()); },
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
                if (_workItemDeletingCommand == null)
                {
                    _workItemDeletingCommand = new RelayCommand(WorkItemDeleting, null);
                }
                return _workItemDeletingCommand;
            }
        }

        /// <summary>
        /// Delete a WorkItem
        /// </summary>
        public void WorkItemDeleting()
        {
            Messenger.Default.Send<AppAction>(AppAction.DELETING_WORK_ITEM);
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
                        () => { Messenger.Default.Send(new WorkItemJournalCreatingAction()); }, 
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
        public void ApplicationClosingNotification()
        {
            Messenger.Default.Send<AppAction>(AppAction.APPLICATION_CLOSING);
        }
    }
}
