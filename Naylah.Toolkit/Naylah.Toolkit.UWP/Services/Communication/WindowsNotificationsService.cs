using NotificationsExtensions.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace Naylah.Toolkit.UWP.Services.Communication
{
    public class WindowsNotificationService
    {

        public bool NotificationsIsAvailble { get { return (Notifier != null) ? Notifier.Setting != NotificationSetting.Enabled : true; } }

        public ToastNotifier Notifier { get; private set; }

        public WindowsNotificationService()
        {

        }

        public async Task Initialize()
        {
            Notifier = ToastNotificationManager.CreateToastNotifier();
        }

        public ToastContent CreateToastContent()
        {
            var toastContent = new ToastContent();
            return toastContent;
        }

        public async Task ShowNotification(
            MessageOptions messageOptions
            )
        {

            var toastContent = CreateToastContent();

            toastContent.Scenario = ToastScenario.Default;

            toastContent.Visual = new ToastVisual();
            toastContent.Visual.TitleText = new ToastText() { Text = messageOptions.Title };

            if (!string.IsNullOrEmpty(messageOptions.BodyText1))
            {
                toastContent.Visual.BodyTextLine1 = new ToastText() { Text = messageOptions.BodyText1 };
            }

            if (!string.IsNullOrEmpty(messageOptions.BodyText2))
            {
                toastContent.Visual.BodyTextLine2 = new ToastText() { Text = messageOptions.BodyText2 };
            }

           

            if (messageOptions.InteractionOptions != null)
            {
                toastContent.Actions = new ToastActionsCustom();

                foreach (var button in messageOptions.InteractionOptions.Buttons)
                {
                    ((ToastActionsCustom)toastContent.Actions).Buttons.Add(new ToastButton(button.Content, button.Argument));
                }
            }

            DateTimeOffset? expiration = null;

            

            if (messageOptions.WindowsNotificationOptions != null)
            {
                toastContent.Audio = new ToastAudio();
                toastContent.Audio.Silent = messageOptions.WindowsNotificationOptions.Silent;

                if (messageOptions.WindowsNotificationOptions.ExpireAfter > 0)
                {
                    expiration = DateTimeOffset.Now.Add(TimeSpan.FromSeconds(messageOptions.WindowsNotificationOptions.ExpireAfter));
                }

            }

            ShowNotificationImmediate(toastContent, expiration, messageOptions?.WindowsNotificationOptions?.Tag, messageOptions.WindowsNotificationOptions?.Group, messageOptions.InteractionOptions?.InteractionEvent);

        }

        private void ShowNotificationImmediate(ToastContent content, DateTimeOffset? expiration = null, string tag = "", string group = "", Action<object> activatedEvent = null)
        {

            var toastNotification = new ToastNotification(content.GetXml());

            if (activatedEvent != null)
            {
                toastNotification.Activated += (s, args) =>
                {
                    var aargs = (ToastActivatedEventArgs)args;
                    activatedEvent?.Invoke(aargs?.Arguments);
                };
            }

            if (!string.IsNullOrEmpty(group))
            {
                toastNotification.Group = group;
            }

            if (!string.IsNullOrEmpty(tag))
            {
                toastNotification.Tag = tag;
            }

            toastNotification.ExpirationTime = expiration;

            Notifier.Show(toastNotification);
        }


    }

    public class WindowsNotificationOptions
    {

        public int ExpireAfter { get; set; }

        public string Group { get; set; }

        public string Tag { get; set; }

        public bool Silent { get; set; }


    }
}
