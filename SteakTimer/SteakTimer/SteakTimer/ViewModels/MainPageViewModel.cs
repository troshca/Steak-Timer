using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteakTimer.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public DelegateCommand OneSteakCommand { get; private set; }
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Количесво стейков";

            OneSteakCommand = new DelegateCommand(Submit);
        }

        void Submit()
        {
            var result = NavigationService.NavigateAsync("NavigationPage/SteakTimerPage");
            if (result.IsFaulted) { System.Diagnostics.Debug.WriteLine(result.Exception.ToString()); }
        }
    }
}
