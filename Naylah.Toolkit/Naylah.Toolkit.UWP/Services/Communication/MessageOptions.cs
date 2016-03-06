using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naylah.Toolkit.UWP.Services.Communication
{
    public class MessageOptions
    {
        public MessageOptions(
            string title,
            string bodyText1,
            string bodyText2,
            bool modal = false,
            InteractionOptions interactionOptions = null,
            WindowsNotificationOptions notificationOptions = null
            )
        {
            Title = title;
            BodyText1 = bodyText1;
            BodyText2 = bodyText2;
            Modal = modal;
            Title = title;
            Title = title;
            InteractionOptions = interactionOptions;
            WindowsNotificationOptions = notificationOptions;
        }

        public string Title { get; set; }

        public string BodyText1 { get; set; }

        public string BodyText2 { get; set; }

        public bool Modal { get; set; }

        public InteractionOptions InteractionOptions { get; set; }

        public WindowsNotificationOptions WindowsNotificationOptions { get; set; }


    }

    public class InteractionOptions
    {


        public InteractionOptions()
        {
            Buttons = new List<Button>();

        }
        public IList<Button> Buttons { get; private set; }

        public Action<object> InteractionEvent { get; set; }


        public class Button
        {

            public Button(string content, string argument)
            {
                this.Content = content;
                this.Argument = argument;
            }

            public string Content { get; set; }
            public string Argument { get; set; }

        }
    }



}
