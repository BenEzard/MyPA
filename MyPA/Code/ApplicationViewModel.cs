using MyPA.Code.Data.Services;
using MyPA.Code.UI.Util;
using System;
using System.Collections.Generic;
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
                    _workItemCreatingCommand = new RelayCommand(BeginWorkItemCreation, CanAddNewWorkItem);
                }
                return _workItemCreatingCommand;
            }
        }

        public void BeginWorkItemCreation()
        {
            Messenger.Default.Send<AppAction>(AppAction.CREATING_WORK_ITEM);
       /*     AppMode = ApplicationMode.ADD_MODE;
            var wi = new WorkItem();
            SelectedWorkItem = wi;*/
            //            WorkItemCreatingEvent?.Invoke(this, new WorkItemEventArgs(WorkItemType.WORK_ITEM_CREATING, wi));
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
            //AppMode = ApplicationMode.EDIT_MODE;
        }

        public bool CanCancelNewWorkItem()
        {
            /* if (AppMode == ApplicationMode.ADD_MODE)
                 return true;
             else
                 return false;*/
            return true;
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
