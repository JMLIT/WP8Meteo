using System;
using System.ComponentModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace WP8Meteo
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual async void NotifyPropertyChanged(string propertyName)
        {
            var propertyChanged = this.PropertyChanged;
           
            if (propertyChanged != null)
            {           
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }
    }
}
