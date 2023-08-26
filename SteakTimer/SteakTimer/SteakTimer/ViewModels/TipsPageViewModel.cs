using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SteakTimer.ViewModels
{
    public class TipsPageViewModel : ViewModelBase
    {
        public TipsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Tips";
        }
    }
}
