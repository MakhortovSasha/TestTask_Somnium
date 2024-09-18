using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask_Somnium.Core;

namespace TestTask_Somnium.MVVM.Model
{
    class ContactModel : ObservableObject
    {

        public string Name { get; set; }
        public string ImageSource { get; set; }
        public MultiThreadObservableCollection<MessageModel> Messages { get; set; } = new MultiThreadObservableCollection<MessageModel>();

        public string UnSendedMessage { get; set; }
        public int UnreadCount => Messages.Where(m=> !m.IsViewed && m.Sender==this).Count();

    }
}
