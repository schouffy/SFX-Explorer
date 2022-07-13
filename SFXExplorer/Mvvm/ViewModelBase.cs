using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SFXExplorer.Mvvm
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region Constructor and Destructor

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        protected ViewModelBase()
        {
        }

#if DEBUG
        /// <summary>
        /// Finalizes an instance of the ViewModelBase class.
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            //string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
            //System.Diagnostics.Debug.WriteLine(msg);
        }
#endif

        #endregion

        #region Public Events

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the parent/master view model of the current view model.
        /// </summary>
        public virtual ViewModelBase MasterViewModel { get; protected set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets a value indicating whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        /// <param name="propertyName">The name of the property to verify</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            //if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            //{
            //    string msg = "Invalid property name: " + propertyName;

            //    if (this.ThrowOnInvalidPropertyName)
            //    {
            //        throw new Exception(msg);
            //    }
            //    else
            //    {
            //        Debug.Assert(false, msg);
            //    }
            //}
        }

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        #endregion
    }
}
