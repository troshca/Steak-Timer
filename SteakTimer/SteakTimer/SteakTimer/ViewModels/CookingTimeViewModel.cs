using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SteakTimer.ViewModels
{
    public class CookingTimeViewModel : ViewModelBase
    {
        public string CookingTimeTitle { get; set; }
        public string Time1 { get; set; }
        public string Time2 { get; set; }
        public string Time3 { get; set; }
        public string Time4 { get; set; }
        public string Time5 { get; set; }
        public string Time6 { get; set; }
        public string Time7 { get; set; }
        public string Time8 { get; set; }

        public string TimeDegr1 { get; set; }
        public string TimeDegr2 { get; set; }
        public string TimeDegr3 { get; set; }
        public string TimeDegr4 { get; set; }
        public string TimeDegr5 { get; set; }



        string _webSiteSource;
        string WebSiteSource
        {
            get { return _webSiteSource; }
            set { SetProperty(ref _webSiteSource, value);  }
        }

        public CookingTimeViewModel(INavigationService navigationService) : base(navigationService)
        {
            CookingTimeTitle = "Как выбрать время прожарки и примерный рецепт:";

            WebSiteSource = "file:///android_asset/htmls/time.html";

            //Strings
            Time1 = "1.\tДостать стейк из холодильника и дать приблизиться к комнатной температуре (около 30 минут);.";
            Time2 = "2.\tУбрать излишнюю влагу и сок с помощью салфетки;";
            Time3 = "3.\tСмазать стейки маслом;";
            Time4 = "4.\tРаскалить сковородку;";
            Time5 = "5.\tВыложить стейк на сковородку;";
            Time6 = "6.\tПрожарка стейка определяется внутренней температурой. \n\r6.1.\tТемпература внутри стейка и примерное время прожарки для стейка толщиной 1,5 см";
            Time7 = "7.\tСнять стейк с горячей поверхности и накрыть крышкой на 5 минут;";
            Time8 = "8.\tПриятного аппетита!";

            TimeDegr1 = "Температура: 39-43°C.\n\rПо 1.5 минуты с каждой стороны на сильном огне.";
            TimeDegr2 = "Температура: 43-47°C.\n\rПо 1-й минуте с каждой стороны на сильном огне и по 1-й минуте на слабом.";
            TimeDegr3 = "Температура: 47-50°C.\n\rПо 1.5-й минуты с каждой стороны на сильном огне и по 2-й минуты на слабом.";
            TimeDegr4 = "Температура: 55-57°C.\n\rПо 1.5-й минуты с каждой стороны на сильном огне и по 3-ы минуте на слабом.";
            TimeDegr5 = "Температура: +60°C.\n\rПо 2-е минуты с каждой стороны на сильном огне и более 3-ы минуте на слабом.";
            //Time6 = "Дать время отдохнуть под крышкой 5 минут";
            //Time7 = "Посолить поперчить перед подачей";
        }
    }
}
