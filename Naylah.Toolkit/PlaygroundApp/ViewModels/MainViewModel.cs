using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;

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



        public ObservableCollection<string> MenuItemsList { get; private set; }

        public MainViewModel()
        {
            CurrentJourney = MainJourneys.None;
            MenuItemsList = new ObservableCollection<string>();
            MenuItemsList.Add("Image Chooser");
            MenuItemsList.Add("");
            MenuItemsList.Add("");
        }

        public async Task SetJourney(string v)
        {
            if (v == "Image Chooser")
            {
                CurrentJourney = MainJourneys.ImageChooser;
            }
            else
            {
                CurrentJourney = MainJourneys.None;
            }
        }
    }

    public enum MainJourneys
    {
        None,
        ImageChooser,

    }
}