using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SteakTimer.ViewModels
{
    public class TipsPageViewModel : ViewModelBase
    {
        public string TipsTitle { get; set; }
        public string Tip1 { get; set; }
        public string Tip2 { get; set; }
        public string Tip3 { get; set; }
        public string Tip4 { get; set; }
        public string Tip5 { get; set; }
        public string Tip6 { get; set; }
        public string Tip7 { get; set; }
        public string Tip8 { get; set; }

        public TipsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Tips";

            TipsTitle = "Советы:";
            //Tips
            Tip1 = "1. Не ополаскивайте и не мойте мясо;";
            Tip2 = "2. Не маринуйте классические стейки;";
            Tip3 = "3. Сковородка должна «пылать»;";
            Tip4 = "4. Не «мучьте» мясо при жарке;";
            Tip5 = "5. Оставьте стейк после приготовления на 5 минут под крышкой;";
            Tip6 = "6. Используйте соль крупного помола;";
        }
    }
}
