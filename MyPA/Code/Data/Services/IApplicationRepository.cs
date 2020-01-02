using System.Collections.Generic;

namespace MyPA.Code.Data.Services
{
    public interface IApplicationRepository
    {
        Dictionary<PreferenceName, Preference> GetApplicationPreferences();
    }
}
