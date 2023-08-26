using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SteakTimer.ViewModels
{
    public class CookingTimeViewModel : ViewModelBase
    {
        public CookingTimeViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Cooking Time";
        }
    }
}
