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
                Console.WriteLine("in command");
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
       /*     AppMode = ApplicationMode.ADD_MODE;

            var wi = new WorkItem();
            SelectedWorkItem = wi;*/
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
    }
}
