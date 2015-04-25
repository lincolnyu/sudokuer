using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sudokuer.Annotations;

namespace Sudokuer.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Properties

        #region INotifyPropertyChanged member

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #endregion

        #region Methods

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
