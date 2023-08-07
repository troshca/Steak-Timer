﻿using LiveChartsCore.Defaults;
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
            for(int i = 10; i <= 10 * 6 * 10; )
            {
                if( i / 60 >= 1 )
                {
                    if((i - (i / 60) * 60) == 0)
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
                .AddValue(FirstCrust, "FirstCrustTime", SKColors.SaddleBrown)
                .AddValue(SecondCrust, "SecondCrustime", SKColors.SaddleBrown)
                .AddValue(FirstFried, "FirstFriedTime", SKColors.SaddleBrown)
                .AddValue(SecondFried, "SecondFriedTime", SKColors.SaddleBrown)
                .BuildSeries();

            //Buttons
            StartCommand = new DelegateCommand(Start);
        }

		public async void Start()
        {
            if (_started) return;
            _started = true;

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
            FirstCrustDiff = _degreesOfCircle / FirstCrustTimeInSeconds / 2;
            SecondCrustDiff = _degreesOfCircle / SecondCrustTimeInSeconds / 2;
            FirstFriedDiff = _degreesOfCircle / FirstFriedTimeInSeconds / 2;
            SecondFriedDiff = _degreesOfCircle / SecondFriedTimeInSeconds / 2;

            for (_currentStage = Stages.FirstCrust; _currentStage <= Stages.SecondFried; _currentStage++)
            {
                switch (_currentStage)
                {
                    case Stages.FirstCrust:
                        {
                            for (;;)
                            {
                                FirstCrust.Value -= FirstCrustDiff;
                                await Task.Delay(500);
                                if (FirstCrust.Value <= 0) 
								{
									FirstCrust.Value = 0;
									break;
								}
                            }
                            if(AddPause)
                                await Task.Delay(PauseTime * 1000);
                            break;
                        }
                    case Stages.SecondCrust:
                        {
                            for (; ; )
                            {
                                SecondCrust.Value -= SecondCrustDiff;
                                await Task.Delay(500);
                                if (SecondCrust.Value <= 0)
								{
									SecondCrust.Value = 0;
									break;
								}
                            }
                            if (AddPause)
                                await Task.Delay(PauseTime * 1000);
                            break;
                        }
                    case Stages.FirstFried:
                        {
                            for (; ; )
                            {
                                FirstFried.Value -= FirstFriedDiff;
                                await Task.Delay(500);
                                if (FirstFried.Value <= 0)
								{
									FirstFried.Value = 0;
									break;
								}
                            }
                            if (AddPause)
                                await Task.Delay(PauseTime * 1000);
                            break;
                        }
                    case Stages.SecondFried:
                        {
                            for (; ; )
                            {
                                SecondFried.Value -= SecondCrustDiff;
                                await Task.Delay(500);
                                if (SecondFried.Value <= 0)
                                {
                                    SecondFried.Value = 0;
                                    break;
                                }
                            }
                            if (AddPause)
                                await Task.Delay(PauseTime * 1000);
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
