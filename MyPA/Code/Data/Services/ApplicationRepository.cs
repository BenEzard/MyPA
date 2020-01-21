using System.Collections.Generic;

namespace MyPA.Code.Data.Services
{
    public class ApplicationRepository : BaseRepository, IApplicationRepository
    {
        Dictionary<PreferenceName, Preference> IApplicationRepository.GetApplicationPreferences()
        {
            return this.GetPreferences("Application");
        }

    }
}
