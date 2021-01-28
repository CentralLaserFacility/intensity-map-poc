// using Prism.Mvvm;

namespace UkCentralLaserPoC.Core.Mvvm
{
    public class ViewModelBase : Prism.Mvvm.BindableBase
    {

        // STEVE_QUESTION - Why this property ??
        // It's set in 'ShellViewModel', but only there.

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
    }
}
