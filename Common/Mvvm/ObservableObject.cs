using System;
using System.ComponentModel;
using System.Diagnostics;
using Common.Utils;

namespace Common.Mvvm
{
    public abstract class ObservableObject : INotifyPropertyChanged, IObservableObject
    {
        #region INotifyPropertyChanged Members


        /// <summary>
        /// Raises the PropertyChange event for the property specified
        /// </summary>
        /// <param name="propertyName">Property name to update. Is case-sensitive.</param>
        /// <param name="verifyPropertyName"></param>
        public void RaisePropertyChanged(string propertyName, bool verifyPropertyName = true)
        {
            if (verifyPropertyName)
            {
                VerifyPropertyName(propertyName);
            }
            OnPropertyChanged(propertyName, verifyPropertyName);
        }


        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        /// <param name="verifyPropertyName"></param>
        protected void OnPropertyChanged(string propertyName, bool verifyPropertyName = true)
        {
            if (verifyPropertyName)
            {
                VerifyPropertyName(propertyName);
            }

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion // INotifyPropertyChanged Members

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                var msg = "Invalid property name: " + propertyName;

                if (ThrowOnInvalidPropertyName)
                    throw new Exception(msg);

                Debug.Fail(msg);
            }
        }


        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might
        /// override this property's getter to return true.
        /// </summary>
        [UsedImplicitly]
        bool ThrowOnInvalidPropertyName { get; set; }
    

        #endregion // Debugging Aides
    }
}
