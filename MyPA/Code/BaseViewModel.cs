using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;

namespace MyPA.Code
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Dictionary<PreferenceName, Preference> Preferences = null;

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
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

        /// <summary>
        /// Return an Application Preference from the collection (in cache), as an int value.
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public int GetAppPreferenceValueAsInt(PreferenceName settingName)
        {
            Preferences.TryGetValue(settingName, out Preference rValue);
            return Int32.Parse(rValue.Value);
        }

        /// <summary>
        /// Return an Application Preference from the collection (in cache), as a double value.
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public double GetAppPreferenceValueAsDouble(PreferenceName settingName)
        {
            Preferences.TryGetValue(settingName, out Preference rValue);
            return Double.Parse(rValue.Value);
        }
    }
}
