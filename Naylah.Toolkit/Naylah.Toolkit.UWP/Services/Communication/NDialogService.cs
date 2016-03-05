using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naylah.Toolkit.UWP.Services.Communication
{
    public class NaylahUWPDialogService
    {

        public bool Initialized { get; private set; }


        public WindowsNotificationService windowsNotificationService { get; private set; }

        public WindowsDialogService windowsDialogService { get; private set; }


        public NDialogServiceApproach Approach { get; set; }


        public NaylahUWPDialogService() : this(new WindowsDialogService(), new WindowsNotificationService())
        {
        }

        public NaylahUWPDialogService(
            WindowsDialogService _windowsDialogService,
            WindowsNotificationService _windowsNotificationService
            )
        {
            windowsDialogService = _windowsDialogService;
            windowsNotificationService = _windowsNotificationService;
        }

        public async Task Initialize()
        {

            await windowsNotificationService.Initialize();

            Initialized = true;
        }




        public async Task ShowMessage(
            MessageOptions messageOptions
            )
        {

            if (!Initialized)
            {
                throw new Exception("NaylahUWPDialogService not initialized");
            }

            if (messageOptions == null)
            {
                throw new ArgumentException("messageOptions is missing");
            }

            if ((Approach == NDialogServiceApproach.MessageDialog) || (windowsNotificationService.NotificationsIsAvailble) || messageOptions.Modal)
            {
                await windowsDialogService.ShowMessage(messageOptions);
                return;
            }
            else
            {
                await windowsNotificationService.ShowNotification(messageOptions);
            }


        }
    }

    public enum NDialogServiceApproach
    {
        Both,
        MessageDialog
    }
}
