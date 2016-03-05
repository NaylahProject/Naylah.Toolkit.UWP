using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Naylah.Toolkit.UWP.Services.Communication
{
    public class WindowsDialogService
    {


        public async Task ShowMessage(
            MessageOptions messageOptions
            )
        {

            if (messageOptions == null)
            {
                throw new ArgumentNullException("messageOptions is null");
            }

            var msgDialog = new MessageDialog(messageOptions.BodyText1);
            msgDialog.Title = messageOptions.Title;

            msgDialog.Content += Environment.NewLine + messageOptions.BodyText2;

            if (messageOptions.InteractionOptions != null)
            {
                foreach (var button in messageOptions.InteractionOptions.Buttons)
                {
                    msgDialog.Commands.Add(new UICommand(button.Content, (s) => { messageOptions.InteractionOptions?.InteractionEvent?.Invoke(s.Id); }, button.Argument));
                }
            }

            await msgDialog.ShowAsync();


        }
    }
}
