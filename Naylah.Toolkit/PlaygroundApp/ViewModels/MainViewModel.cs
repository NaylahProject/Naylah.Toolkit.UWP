using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using Naylah.Toolkit.UWP.Services.Communication;
using System.Globalization;

namespace PlaygroundApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        private double _myDouble;

        public double MyDouble
        {
            get { return _myDouble; }
            set { Set(ref _myDouble, value); }
        }




        private MainJourneys _currentJourney;

        public MainJourneys CurrentJourney
        {
            get { return _currentJourney; }
            set
            {
                Set(ref _currentJourney, value);
                RaisePropertyChanged(() => this.ImageChooserJourney);
                RaisePropertyChanged(() => this.DialogServiceJourney);
                RaisePropertyChanged(() => this.BehaviorsJourney);
            }
        }




        public bool ImageChooserJourney { get { return CurrentJourney == MainJourneys.ImageChooser; } }
        public bool DialogServiceJourney { get { return CurrentJourney == MainJourneys.DialogService; } }
        public bool BehaviorsJourney { get { return CurrentJourney == MainJourneys.Behaviors; } }



        public ObservableCollection<string> MenuItemsList { get; private set; }

        public NaylahUWPDialogService DialogService { get; private set; }

        public MainViewModel()
        {
            CurrentJourney = MainJourneys.Behaviors;
            MenuItemsList = new ObservableCollection<string>();
            MenuItemsList.Add("None");
            MenuItemsList.Add("Image Chooser");
            MenuItemsList.Add("Dialog Service");
            MenuItemsList.Add("Behaviors");

            DialogService = new Naylah.Toolkit.UWP.Services.Communication.NaylahUWPDialogService();
            DialogService.Initialize();

            MyDouble = 8.9;

            //CultureInfo.CurrentCulture = new CultureInfo("pt-BR");

        }

        public async Task SetJourney(string v)
        {
            if (v == "Image Chooser")
            {
                CurrentJourney = MainJourneys.ImageChooser;
            }
            else
            {
                if (v == "Dialog Service")
                {
                    CurrentJourney = MainJourneys.DialogService;
                }
                else
                {
                    if (v == "Behaviors")
                    {
                        CurrentJourney = MainJourneys.Behaviors;
                    }
                    else
                    {
                        CurrentJourney = MainJourneys.None;
                    }
                }

            }
        }

        internal void ShowDialog(string v)
        {
            //var interaction = new InteractionOptions();

            //interaction.Buttons.Add(new InteractionOptions.Button("Sim", "s"));
            //interaction.Buttons.Add(new InteractionOptions.Button("Nao", "n"));

            //interaction.InteractionEvent += asdas;

            var messageOptions = new MessageOptions("CultureInfo Infots", CultureInfo.CurrentCulture.DisplayName + Environment.NewLine + CultureInfo.CurrentUICulture.DisplayName, null, true, null, new WindowsNotificationOptions() { ExpireAfter = 10 });

            DialogService.ShowMessage(messageOptions);
        }

        private void asdas(object obj)
        {

        }

    }

    public enum MainJourneys
    {
        None,
        ImageChooser,
        DialogService,
        Behaviors,
    }
}