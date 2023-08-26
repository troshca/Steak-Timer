using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Prism.Commands;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;
using SkiaSharp;
using SteakTimer.Models;
using Acr.UserDialogs;
using Xamarin.Essentials;
using System.Reflection;
using Plugin.LocalNotification;
using Prism.AppModel;
using Prism.Navigation.Xaml;

namespace SteakTimer.ViewModels
{
    public class SteakTimerViewModel : ViewModelBase, IApplicationLifecycleAware
    {
        //For Application Lifecycle Management
        private DateTime _timeOnSleep { get; set; }
        private DateTime _timeOnResume { get; set; }
        private TimeSpan _difference { get; set; }
        private bool _wasResume { get; set; }
        private bool _wasSleep { get; set; }
        private bool _abort { get; set; }
        public DelegateCommand StartCommand { get; private set; }
        public DelegateCommand AbortCommand { get; private set; }
        public DelegateCommand InfoCommand { get; private set; }

        //
        private string timeLeft;
        public string TimeLeft
        {
            get { return timeLeft; }
            set { SetProperty(ref timeLeft, value); }
        }

        //
        enum Stages
        {
            FirstCrust,
            FirstFlip,
            SecondCrust,
            SecondFlip,
            FirstFried,
            ThirdFlip,
            SecondFried,
            End
        }
        private Stages _currentStage { get; set; }
        private bool _started;
        public bool Started
        {
            get { return _started; }
            set { SetProperty(ref _started, value); }
        }
        private int _pauseTime;
        public int PauseTime
        {
            get { return _pauseTime; }
            set { SetProperty(ref _pauseTime, value); }
        }
        private bool _addPause;
        public bool AddPause
        {
            get { return _addPause; }
            set { SetProperty(ref _addPause, value); }
        }

        private int _degreesOfCircle { get; set; }
        public ObservableValue FirstCrust { get; set; }
        public int FirstCrustTimeInSeconds { get; set; }
        public double FirstCrustDiff { get; set; }
        public ObservableValue SecondCrust { get; set; }
        public int SecondCrustTimeInSeconds { get; set; }
        public double SecondCrustDiff { get; set; }
        public ObservableValue FirstFried { get; set; }
        public int FirstFriedTimeInSeconds { get; set; }
        public double FirstFriedDiff { get; set; }
        public ObservableValue SecondFried { get; set; }
        public int SecondFriedTimeInSeconds { get; set; }
        public double SecondFriedDiff { get; set; }
        public IEnumerable<ISeries> Series { get; set; }

        //Pickers
        private List<MySpecialTime> _timesForPickers { get; set; }
        private List<MySpecialTime> _crustTimePickerCollection;
        public List<MySpecialTime> CrustTimePickerCollection
        {
            get { return _crustTimePickerCollection; }
            set { SetProperty(ref _crustTimePickerCollection, value); }
        }

        private MySpecialTime _pickerCrustSelection;
        public MySpecialTime PickerCrustSelection
        {
            get { return _pickerCrustSelection; }
            set { SetProperty(ref _pickerCrustSelection, value); }
        }
        private List<MySpecialTime> _friedTimePickerCollection;
        public List<MySpecialTime> FriedTimePickerCollection
        {
            get { return _friedTimePickerCollection; }
            set { SetProperty(ref _friedTimePickerCollection, value); }
        }
        private MySpecialTime _pickerFriedSelection;
        public MySpecialTime PickerFriedSelection
        {
            get { return _pickerFriedSelection; }
            set { SetProperty(ref _pickerFriedSelection, value, () => RaisePropertyChanged(nameof(FirstCrust))); }
        }

        public SteakTimerViewModel(INavigationService navigationService) : base(navigationService)
        {
            //Picker
            _timesForPickers = new List<MySpecialTime>();
            for (int i = 10; i <= 10 * 6 * 10;)
            {
                if (i / 60 >= 1)
                {
                    if ((i - (i / 60) * 60) == 0)
                    {
                        _timesForPickers.Add(new MySpecialTime((i / 60).ToString() + " мин. ", i));
                    }
                    else
                    {
                        _timesForPickers.Add(new MySpecialTime((i / 60).ToString() + " мин. " + ((i - (i / 60) * 60)).ToString() + " сек.", i));
                    }
                }
                else
                {
                    _timesForPickers.Add(new MySpecialTime(i.ToString() + " сек.", i));
                }
                i += 10;
            }

            //Fill pickers
            CrustTimePickerCollection = _timesForPickers;
            FriedTimePickerCollection = _timesForPickers;

            //Select default
            PickerCrustSelection = _timesForPickers[5];
            PickerFriedSelection = _timesForPickers[8];

            //
            TimeLeft = "< " + ((PickerCrustSelection.Value * 2) / 60 + (PickerFriedSelection.Value * 2) / 60).ToString() + "мин.";
            //Interval = TimeSpan.FromMilliseconds(500);

            //Degrees of Circle
            _degreesOfCircle = 360;

            //Degrees
            FirstCrust = new ObservableValue { Value = _degreesOfCircle };
            SecondCrust = new ObservableValue { Value = _degreesOfCircle };
            FirstFried = new ObservableValue { Value = _degreesOfCircle };
            SecondFried = new ObservableValue { Value = _degreesOfCircle };

            //Pause
            AddPause = true;
            PauseTime = 5;

            //Chart
            Series = new GaugeBuilder()
                .WithOffsetRadius(5)
                .WithLabelsSize(0)
                .WithInnerRadius(75)
                .AddValue(FirstCrust, "Первая сторона корочки", SKColors.DarkRed)
                .AddValue(SecondCrust, "Вторая сторона корочки", SKColors.IndianRed)
                .AddValue(FirstFried, "Первая сторона прожарки", SKColors.Brown)
                .AddValue(SecondFried, "Вторая сторона прожарки", SKColors.SaddleBrown)
                .BuildSeries();

            //Buttons
            StartCommand = new DelegateCommand(async () => await StartJob());
            AbortCommand = new DelegateCommand(async () => await AbortJob());
            InfoCommand = new DelegateCommand(() => Info());

            _abort = false;
            _wasResume = false;
        }

        private Task AbortJob()
        {
            _abort = true;
            _currentStage = Stages.End;
            LocalNotificationCenter.Current.ClearAll();
            LocalNotificationCenter.Current.CancelAll();
            return Task.CompletedTask;
        }

        void Info()
        {
            var result = NavigationService.NavigateAsync("NavigationPage/InfoPage", useModalNavigation:true, animated:true);
            if (result.IsFaulted) { System.Diagnostics.Debug.WriteLine(result.Exception.ToString()); }
        }

        private void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
            });
        }

        public async void Start()
        {

            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            if (Started) return;
            Started = true;

            DeviceDisplay.KeepScreenOn = true;

            FirstCrust.Value = _degreesOfCircle;
            SecondCrust.Value = _degreesOfCircle;
            FirstFried.Value = _degreesOfCircle;
            SecondFried.Value = _degreesOfCircle;

            //Times
            FirstCrustTimeInSeconds = PickerCrustSelection.Value;
            SecondCrustTimeInSeconds = PickerCrustSelection.Value;
            FirstFriedTimeInSeconds = PickerFriedSelection.Value;
            SecondFriedTimeInSeconds = PickerFriedSelection.Value;

            //Degrees per second
            FirstCrustDiff = (double)_degreesOfCircle / FirstCrustTimeInSeconds;
            SecondCrustDiff = (double)_degreesOfCircle / SecondCrustTimeInSeconds;
            FirstFriedDiff = (double)_degreesOfCircle / FirstFriedTimeInSeconds;
            SecondFriedDiff = (double)_degreesOfCircle / SecondFriedTimeInSeconds;

            //Messages
            string title = $"Переворачивай";
            string message = $"Переворачивай свой стейк пока не сгорел!";

            //Config all notifications
            var timeToAlarm = DateTime.Now.AddSeconds(FirstCrustTimeInSeconds);
            System.Diagnostics.Debug.WriteLine(timeToAlarm);

            var notificationFirstCrust = new NotificationRequest
            {
                NotificationId = 100,
                Title = title,
                Schedule =
                {
                    NotifyTime = timeToAlarm
                }
            };
            timeToAlarm = timeToAlarm.AddSeconds(SecondCrustTimeInSeconds);
            if(AddPause)
            { timeToAlarm = timeToAlarm.AddSeconds(5); }
            var notificationSecondCrust = new NotificationRequest
            {
                NotificationId = 101,
                Title = title,
                Schedule =
                {
                    NotifyTime = timeToAlarm
                }
            };
            timeToAlarm = timeToAlarm.AddSeconds(FirstFriedTimeInSeconds);
            if (AddPause)
            { timeToAlarm = timeToAlarm.AddSeconds(5); }
            var notificationFirstFried = new NotificationRequest
            {
                NotificationId = 102,
                Title = title,
                Schedule =
                {
                    NotifyTime = timeToAlarm
                }
            };
            timeToAlarm = timeToAlarm.AddSeconds(SecondFriedTimeInSeconds);
            if (AddPause)
            { timeToAlarm = timeToAlarm.AddSeconds(5); }
            var notificationSecondFried = new NotificationRequest
            {
                NotificationId = 103,
                Title = title,
                Description = "Приятного аппетита",
                Schedule =
                {
                    NotifyTime = timeToAlarm,
                }
            };

            //Showing notifications
            _ = LocalNotificationCenter.Current.Show(notificationFirstCrust);
            _ = LocalNotificationCenter.Current.Show(notificationSecondCrust);
            _ = LocalNotificationCenter.Current.Show(notificationFirstFried);
            _ = LocalNotificationCenter.Current.Show(notificationSecondFried);

            //Toasts config (delete?)
            var toastConfig = new ToastConfig(title);
            toastConfig.BackgroundColor = Color.White;
            toastConfig.MessageTextColor = Color.Black;
            toastConfig.Position = ToastPosition.Bottom;
            toastConfig.Duration = TimeSpan.FromSeconds(5);

            //TimeSpans
            TimeSpan _timeSpanFirstCrust = TimeSpan.FromSeconds(FirstCrustTimeInSeconds);
            TimeSpan _timeSpanSecondCrust = TimeSpan.FromSeconds(SecondCrustTimeInSeconds);
            TimeSpan _timeSpanFirstFried = TimeSpan.FromSeconds(FirstFriedTimeInSeconds);
            TimeSpan _timeSpanSecondFried = TimeSpan.FromSeconds(SecondCrustTimeInSeconds);
            TimeSpan _timeSpanPause = TimeSpan.FromSeconds(PauseTime);

            _wasSleep = false;
            _wasResume = false;

            for (_currentStage = Stages.FirstCrust; _currentStage < Stages.End; _currentStage++)
            {
                if(_abort)
                { break; }
                switch (_currentStage)
                {

                    case Stages.FirstCrust:
                        {
                            for ( ; ; )
                            {
                                do
                                {
                                    await Task.Delay(1);
                                } while (_wasSleep);

                                if (_wasResume)
                                {
                                    if((FirstCrust.Value) < (_difference.TotalSeconds * FirstCrustDiff))
                                    {
                                        _difference = _difference.Subtract(_timeSpanFirstCrust);
                                        FirstCrust.Value = 0;
                                        break;
                                    }
                                    else
                                    {
                                        _wasResume = false;
                                        FirstCrust.Value = FirstCrust.Value - _difference.TotalSeconds * FirstCrustDiff;
                                    }
                                }
                                FirstCrust.Value -= FirstCrustDiff;
                                if (FirstCrust.Value <= 0)
                                {
                                    FirstCrust.Value = 0;
                                    break;
                                }
                                await Task.Delay(1000);
                            }
                            //_ = UserDialogs.Instance.Toast(toastConfig);
                            //notificationManager.SendNotification(title, message);

                            break;
                        }

                    case Stages.SecondCrust:
                        {
                            for ( ; ; )
                            {
                                do
                                {
                                    await Task.Delay(1);
                                } while (_wasSleep);

                                if (_wasResume)
                                {
                                    if (SecondCrust.Value < (_difference.TotalSeconds * SecondFriedDiff))
                                    {
                                        _difference.Subtract(_timeSpanSecondCrust);
                                        SecondCrust.Value = 0;
                                        break;
                                    }
                                    else
                                    {
                                        _wasResume = false;
                                        SecondCrust.Value = SecondCrust.Value - _difference.TotalSeconds * SecondCrustDiff;
                                    }
                                }
                                SecondCrust.Value -= SecondCrustDiff;
                                if (SecondCrust.Value <= 0)
                                {
                                    SecondCrust.Value = 0;
                                    break;
                                }
                                await Task.Delay(1000);
                            }
                            //_ = UserDialogs.Instance.Toast(toastConfig);
                            //notificationManager.SendNotification(title, message);

                            break;
                        }

                    case Stages.FirstFried:
                        {
                            for ( ; ; )
                            {
                                do
                                {
                                    await Task.Delay(1);
                                } while (_wasSleep);

                                if (_wasResume)
                                {
                                    if (FirstFried.Value < (_difference.TotalSeconds * FirstFriedDiff))
                                    {
                                        _difference= _difference.Subtract(_timeSpanFirstFried);
                                        FirstFried.Value = 0;
                                        break;
                                    }
                                    else
                                    {
                                        _wasResume = false;
                                        FirstFried.Value = FirstFried.Value - _difference.TotalSeconds * FirstFriedDiff;
                                    }
                                }
                                FirstFried.Value -= FirstFriedDiff;
                                if (FirstFried.Value <= 0)
                                {
                                    FirstFried.Value = 0;
                                    break;
                                }
                                await Task.Delay(1000);
                            }
                            //_ = UserDialogs.Instance.Toast(toastConfig);
                            //notificationManager.SendNotification(title, message);

                            break;
                        }

                    case Stages.SecondFried:
                        {
                            for ( ; ; )
                            {
                                do
                                {
                                    await Task.Delay(1);
                                } while (_wasSleep);

                                if (_wasResume)
                                {
                                    if (SecondFried.Value < (_difference.TotalSeconds * SecondFriedDiff))
                                    {
                                        FirstFried.Value = 0;
                                        break;
                                    }
                                    else
                                    {
                                        _wasResume = false;
                                        SecondFried.Value = SecondFried.Value - _difference.TotalSeconds * SecondFriedDiff;
                                    }
                                }
                                SecondFried.Value -= SecondCrustDiff;
                                if (SecondFried.Value <= 0)
                                {
                                    SecondFried.Value = 0;
                                    break;
                                }
                                await Task.Delay(1000);
                            }
                            //title = $"Снимай!";
                            //message = $"Приятного аппетита";
                            //toastConfig.Message = title + message;
                            //_ = UserDialogs.Instance.Toast(toastConfig);
                            //notificationManager.SendNotification(title, message);
                            break;
                        }
                    default:
                        {
                            if (AddPause)
                            {
                                do
                                {
                                    await Task.Delay(1);
                                } while (_wasSleep);

                                if (_wasResume)
                                {
                                    if (_difference.TotalSeconds < PauseTime)
                                    {
                                        _wasResume = false;
                                        await Task.Delay((int)(PauseTime - _difference.TotalSeconds) * 1000);
                                        break;
                                    }
                                    else
                                    {
                                        _difference = _difference.Subtract(_timeSpanPause);
                                        break;
                                    }
                                }
                                else
                                {
                                    await Task.Delay(PauseTime * 1000);
                                }
                            }
                            break;
                        }
                }
            }
            Started = false;
            _wasResume = false;
            FirstCrust.Value = _degreesOfCircle;
            SecondCrust.Value = _degreesOfCircle;
            FirstFried.Value = _degreesOfCircle;
            SecondFried.Value = _degreesOfCircle;
            await Task.Delay(500);

            DeviceDisplay.KeepScreenOn = false;
        }

        public Task<bool> StartJob()
        {
            Start();
            return Task.FromResult(false);
        }

        //For Application Lifecycle Management
        public void OnResume()
        {
            if(Started)
            {
                _wasResume = true;
                _wasSleep = false;
                _timeOnResume = DateTime.Now;
                _difference = _timeOnResume.Subtract(_timeOnSleep);
            }
        }

        public void OnSleep()
        {
            if(Started)
            {
                _wasResume = false;
                _wasSleep = true;
                _timeOnSleep = DateTime.Now; 
            }
        }
    }
}
