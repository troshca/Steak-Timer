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

namespace SteakTimer.ViewModels
{
    public class SteakTimerViewModel : ViewModelBase
    {
        public DelegateCommand StartCommand { get; private set; }


        private readonly Random _random = new Random();
        
        //
        enum Stages
        {
            FirstCrust,
            SecondCrust,
            FirstFried,
            SecondFried,
            End
        }
        private Stages _currentStage { get; set; }
        private bool _started {  get; set; }

        private int _degreesOfCircle {  get; set; }
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

        public SteakTimerViewModel(INavigationService navigationService) : base(navigationService)
        {
            //Degrees of Circle
            _degreesOfCircle = 360;

            //Times
            FirstCrustTimeInSeconds = 60;
            SecondCrustTimeInSeconds = 60;
            FirstFriedTimeInSeconds = 90;
            SecondFriedTimeInSeconds = 90;

            //Degrees per second
            FirstCrustDiff = _degreesOfCircle / FirstCrustTimeInSeconds;
            SecondCrustDiff = _degreesOfCircle / SecondCrustTimeInSeconds;
            FirstFriedDiff = _degreesOfCircle / FirstFriedTimeInSeconds;
            SecondFriedDiff = _degreesOfCircle / SecondFriedTimeInSeconds;

            //Degrees
            FirstCrust = new ObservableValue { Value = _degreesOfCircle };
            SecondCrust = new ObservableValue { Value = _degreesOfCircle };
            FirstFried = new ObservableValue { Value = _degreesOfCircle };
            SecondFried = new ObservableValue { Value = _degreesOfCircle };

            //
            Series = new GaugeBuilder()
                .WithOffsetRadius(5)
                .WithLabelsSize(0)
                .AddValue(FirstCrust, "FirstCrustTime", SKColors.SaddleBrown)
                .AddValue(SecondCrust, "SecondCrustime", SKColors.SaddleBrown)
                .AddValue(FirstFried, "FirstFriedTime", SKColors.SaddleBrown)
                .AddValue(SecondFried, "SecondFriedTime", SKColors.SaddleBrown)
                .BuildSeries();

            StartCommand = new DelegateCommand(Start);
        }

		public async void Start()
        {
            if (_started) return;
            _started = true;
            for (_currentStage = Stages.FirstCrust; _currentStage <= Stages.SecondFried; _currentStage++)
            {
                switch (_currentStage)
                {
                    case Stages.FirstCrust:
                        {
                            for (;;)
                            {
                                FirstCrust.Value -= FirstCrustDiff;
                                await Task.Delay(1000);
                                if (FirstCrust.Value <= 0) 
								{
									FirstCrust.Value = 0;
									break;
								}
                            }
                            break;
                        }
                    case Stages.SecondCrust:
                        {
                            for (; ; )
                            {
                                SecondCrust.Value -= SecondCrustDiff;
                                await Task.Delay(1000);
                                if (SecondCrust.Value <= 0)
								{
									SecondCrust.Value = 0;
									break;
								}
                            }
                            break;
                        }
                    case Stages.FirstFried:
                        {
                            for (; ; )
                            {
                                FirstFried.Value -= FirstFriedDiff;
                                await Task.Delay(1000);
                                if (FirstFried.Value <= 0)
								{
									FirstFried.Value = 0;
									break;
								}
                            }
                            break;
                        }
                    case Stages.SecondFried:
                        {
                            for (; ; )
                            {
                                SecondFried.Value -= SecondCrustDiff;
                                await Task.Delay(1000);
                                if (SecondFried.Value <= 0)
                                {
                                    SecondFried.Value = 0;
                                    break;
                                }
                            }
                            break;
                        }
                }
            }
            _started = false;
        }

        private async Task SecondsTickTask()
        {
            while (true)
            {
                await Task.Delay(1000);
            }
        }

        private int FindMax(params int[] values)
        {
            return Enumerable.Max(values);
        }
    }
}
