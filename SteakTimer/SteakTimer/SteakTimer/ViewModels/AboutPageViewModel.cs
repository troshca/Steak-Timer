using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Acr.UserDialogs;

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

        public DelegateCommand MailCommand { get; private set; }
        public DelegateCommand TelegramCommand { get; private set; }
        public DelegateCommand GitHubCommand { get; private set; }

        public AboutPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "About";
            LabelAboutText = "Спасибо, что установили приложение \n\r Steak Timer!";
            AboutText = "Буду благодарен обратной связи.\n\r О возможных проблемах \n\rпрошу сообщить.";

            //Commands
            MailCommand = new DelegateCommand(() => MailAsync());
            TelegramCommand = new DelegateCommand(() => TelegramAsync());
            GitHubCommand = new DelegateCommand(() => GitHubAsync());
        }

        private async Task GitHubAsync()
        {
            try
            {
                await Browser.OpenAsync("https://github.com/troshca/Steak-Timer", BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private async Task TelegramAsync()
        {
            try
            {
                await Browser.OpenAsync("https://t.me/sergeikliuzhin", BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private async Task MailAsync()
        {
            var recipients = new List<string>();
            recipients.Add("sergeyklyuzhin@gmail.com");
            try
            {
                var message = new EmailMessage
                {
                    Subject = "Steak Timer",
                    Body = "",
                    To = recipients,
                    //Cc = ccRecipients,
                    //Bcc = bccRecipients
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                System.Diagnostics.Debug.WriteLine(fbsEx.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
