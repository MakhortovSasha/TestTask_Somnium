using TestTask_Somnium.Core;

namespace TestTask_Somnium.MVVM.Model
{
    class MessageModel : ObservableObject
    {
        public ContactModel Sender { get; set; }

        public string Message { get; set; }

        public DateTime WhenSended { get; set; }

        public bool External { get; set; } = true;


        public bool IsViewed { get; set; } = false;


        public string Subject => Sender.Name + "-" + ((WhenSended.Date == DateTime.Now.Date) ? WhenSended.ToString("t") : WhenSended.ToString("d MMMM HH:mm"));
    }
}