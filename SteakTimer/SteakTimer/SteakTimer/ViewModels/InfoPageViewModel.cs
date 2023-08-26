using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SteakTimer.ViewModels
{
    class InfoPageViewModel : ViewModelBase
    {
        public DelegateCommand AboutCommand { get; private set; }
        public DelegateCommand CoockingTimeCommand { get; private set; }
        public DelegateCommand TipsCommand { get; private set; }
        public InfoPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Полезное";

            //Commands
            AboutCommand = new DelegateCommand(() => About());
            CoockingTimeCommand = new DelegateCommand(() => CookingTime());
            TipsCommand = new DelegateCommand(() => Tips());
        }

        void About()
        {
            var result = NavigationService.NavigateAsync("NavigationPage/AboutPage", useModalNavigation: true, animated: true);
            if (result.IsFaulted) { System.Diagnostics.Debug.WriteLine(result.Exception.ToString()); }
        }
        void CookingTime()
        {
            var result = NavigationService.NavigateAsync("NavigationPage/CookingTimePage", useModalNavigation: true, animated: true);
            if (result.IsFaulted) { System.Diagnostics.Debug.WriteLine(result.Exception.ToString()); }
        }
        void Tips()
        {
            var result = NavigationService.NavigateAsync("NavigationPage/TipsPage", useModalNavigation: true, animated: true);
            if (result.IsFaulted) { System.Diagnostics.Debug.WriteLine(result.Exception.ToString()); }
        }
    }
}
