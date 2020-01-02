using MyPA.Code.Data.Services;
using System.Collections.Generic;

namespace MyPA.Code
{
    public class ApplicationViewModel : BaseViewModel
    {
        private IApplicationRepository applicationRepository = new ApplicationRepository();
        public Dictionary<PreferenceName, Preference> Preferences = null;

        public void LoadApplicationPreferences()
        {
            Preferences = applicationRepository.GetApplicationPreferences();
        }
    }
}
