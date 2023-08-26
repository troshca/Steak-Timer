using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SteakTimer.ViewModels
{
    public class AboutPageViewModel : ViewModelBase
    {
        private string _labelAboutText;
        public string LabelAboutText
        {
            get { return _labelAboutText; } 
            set { SetProperty(ref _labelAboutText, value); }
        }
        private string _AboutText;
        public string AboutText
        {
            get { return _AboutText; }
            set { SetProperty(ref _AboutText, value); }
        }

        public AboutPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "About";
            LabelAboutText = "Спасибо, что установили приложение Steak Timer";
            AboutText = "Буду благодарен обратной связи. О возможных проблем прошу написать.";
        }
    }
}
