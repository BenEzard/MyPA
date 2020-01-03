using MyPA.Code.Data.Services;
using System.Collections.Generic;

namespace MyPA.Code
{
    public class ApplicationViewModel : BaseViewModel
    {
        private IApplicationRepository applicationRepository = new ApplicationRepository();
        public Dictionary<PreferenceName, Preference> Preferences = null;

        public ApplicationViewModel()
        {
            LoadApplicationPreferences();
        }

        public void LoadApplicationPreferences()
        {
            Preferences = applicationRepository.GetApplicationPreferences();
        }

        /// <summary>
        /// Return an Application Preference from the collection (in cache), as a string value.
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public string GetAppPreferenceValue(PreferenceName settingName)
        {
            Preferences.TryGetValue(settingName, out Preference rValue);
            return rValue.Value;
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
                return int.Parse(GetAppPreferenceValue(PreferenceName.APPLICATION_WIDTH));
            }
        }

        public int ApplicationHeight
        {
            get
            {
                return int.Parse(GetAppPreferenceValue(PreferenceName.APPLICATION_HEIGHT));
            }
        }

        public int ApplicationPositionTop
        {
            get
            {
                return int.Parse(GetAppPreferenceValue(PreferenceName.APPLICATION_POSITION_TOP));
            }
        }

        public int ApplicationPositionLeft
        {
            get
            {
                return int.Parse(GetAppPreferenceValue(PreferenceName.APPLICATION_POSITION_LEFT));
            }
        }
    }
}
