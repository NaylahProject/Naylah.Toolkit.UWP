using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using Naylah.Toolkit.UWP.Services.Communication;

namespace PlaygroundApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        private MainJourneys _currentJourney;

        public MainJourneys CurrentJourney
        {
            get { return _currentJourney; }
            set
            {
                Set(ref _currentJourney, value);
                RaisePropertyChanged(() => this.ImageChooserJourney);
            }
        }


       

        public bool ImageChooserJourney { get { return CurrentJourney == MainJourneys.ImageChooser; } }
        public bool DialogServiceJourney { get { return CurrentJourney == MainJourneys.DialogService; } }



        public ObservableCollection<string> MenuItemsList { get; private set; }

        public NaylahUWPDialogService DialogService { get; private set; }

        public MainViewModel()
        {
            CurrentJourney = MainJourneys.DialogService;
            MenuItemsList = new ObservableCollection<string>();
            MenuItemsList.Add("None");
            MenuItemsList.Add("Image Chooser");
            MenuItemsList.Add("Dialog Service");

            DialogService = new Naylah.Toolkit.UWP.Services.Communication.NaylahUWPDialogService();
            DialogService.Approach = NDialogServiceApproach.MessageDialog;
            DialogService.Initialize();


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
                    CurrentJourney = MainJourneys.None;
                }
                
            }
        }

        internal void ShowDialog(string v)
        {
            var messageOptions = new MessageOptions("Clima", "Vai chover" ,"Vc vai sair com guarda-chuva?" );
            messageOptions.WindowsNotificationOptions = new WindowsNotificationOptions();
            messageOptions.WindowsNotificationOptions.ExpireAfter = 10;
            messageOptions.WindowsNotificationOptions.Silent = true;
            //messageOptions.Modal = true;
            //messageOptions.InteractionOptions = new InteractionOptions();
            //messageOptions.InteractionOptions.Buttons.Add(new InteractionOptions.Button("Sim", "S"));
            //messageOptions.InteractionOptions.Buttons.Add(new InteractionOptions.Button("Nao", "N"));
            //messageOptions.InteractionOptions.InteractionEvent = Obj;
            DialogService.ShowMessage(messageOptions);
        }

        private void Obj(object obj)
        {
            
        }
    }

    public enum MainJourneys
    {
        None,
        ImageChooser,
        DialogService,
    }
}