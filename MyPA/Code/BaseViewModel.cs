using MyPA.Code.Data.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MyPA.Code
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Dictionary<PreferenceName, Preference> Preferences = null;

        public delegate void ModelEventHandler(object obj, BaseNotification e);
        public event ModelEventHandler ModelEvent;

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void InvokeEvent(object obj, BaseNotification action)
        {
            ModelEvent?.Invoke(obj, action);
        }

        #region AppPreferences
        /// <summary>
        /// Return an Application Preference from the collection (in cache), as a string value.
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public string GetAppPreferenceValue(PreferenceName settingName)
        {
            Preferences.TryGetValue(settingName, out Preference rValue);
            if (rValue == null)
                throw new ArgumentException($"Cannot find Preference: {settingName}");
            return rValue.Value;
        }

        /// <summary>
        /// Return a Preference from the collection (in cache), as an int value.
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public int GetAppPreferenceValueAsInt(PreferenceName settingName)
        {
            Preferences.TryGetValue(settingName, out Preference rValue);
            if (rValue == null)
                throw new ArgumentException($"Cannot find Preference: {settingName}");
            return Int32.Parse(rValue.Value);
        }

        /// <summary>
        /// Return a Preference from the collection (in cache), as a bool value.
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public bool GetAppPreferenceValueAsBool(PreferenceName settingName)
        {
            bool returnValue;
            Preferences.TryGetValue(settingName, out Preference rValue);
            if (rValue == null)
                throw new ArgumentException($"Cannot find Preference: {settingName}");
            if (rValue.Value.Equals("1"))
                returnValue = true;
            else
                returnValue = false;

            return returnValue;
        }

        /// <summary>
        /// Return a Preference from the collection (in cache), as a double value.
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public double GetAppPreferenceValueAsDouble(PreferenceName settingName)
        {
            Preferences.TryGetValue(settingName, out Preference rValue);
            if (rValue == null)
                throw new ArgumentException($"Cannot find Preference: {settingName}");
            return Double.Parse(rValue.Value);
        }
        #endregion
    }
}
