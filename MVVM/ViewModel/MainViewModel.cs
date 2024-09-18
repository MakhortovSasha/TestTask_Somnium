using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TestTask_Somnium.Core;
using TestTask_Somnium.Core.Helpers;
using TestTask_Somnium.MVVM.Model;

namespace TestTask_Somnium.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        
        private RandomChatBot chatBotOnBackground;

        private ContactModel _currentContact;
        private bool _typingEnabled =false;

        private bool selectionChangedTrigger= false;



        #region Properties


        public string Title { get; private set; } = "Real-time chats with online users";
        public ContactModel Me { get; set; }

        public bool SelectionChangedTrigger
        {
            get { return selectionChangedTrigger; }
            set 
            {
                selectionChangedTrigger = value;
                OnPropertyChanged(nameof(SelectionChangedTrigger));
            }
        } 

        public ObservableCollection<ContactModel> Contacts { get; set; }
        public MultiThreadObservableCollection<MessageModel> Messages
        {
            get
            {
                if (_currentContact != null)
                    return _currentContact.Messages;
                return null;
            }
            set
            {
                _currentContact.Messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public ContactModel SelectedContact
        {
            get
            {
                return _currentContact;
            }
            set
            {
                SelectionChangedTrigger = false;
                _currentContact = value;
                OnPropertyChanged("SelectedContact");

                TypingEnabled = (_currentContact!=null);

                Messages = value.Messages;
                TypedMessage = value.UnSendedMessage;

                foreach (var message in value.Messages.Where(m => !m.IsViewed && m.Sender != Me))
                { 
                    message.IsViewed = true;
                    message.OnPropertyChanged(nameof(message.IsViewed));

                }

                SelectionChangedTrigger = true;

                SelectedContact.OnPropertyChanged("UnreadCount");


            }
        }
        public bool TypingEnabled
        {
            get
            { return _typingEnabled; }
            set
            {
                _typingEnabled = value;
                OnPropertyChanged(nameof(TypingEnabled));
            }
        }
         
        public string TypedMessage
        {
            get
            {
                if (_currentContact != null)
                {
                    if (string.IsNullOrEmpty(_currentContact.UnSendedMessage))
                        _currentContact.UnSendedMessage =
                            @"<FlowDocument xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""></FlowDocument>";
                    return _currentContact.UnSendedMessage;
                }
                else return null;
            }
            set
            {               
                    _currentContact.UnSendedMessage = value;
                    OnPropertyChanged(nameof(TypedMessage));
            }
        }


        #region Commands
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand OpenEmojiPickerCommand {  get; set; }

        #endregion Commands
        #endregion Propertiew

        public void RecieveMessage(MessageModel message)
        {
            if (message == null|| message.Sender==null||string.IsNullOrWhiteSpace(message.Message.RemoveXMLMarkup())) return;


            if (message.Sender == Me)
                message.External = false;

            if (message.Sender != SelectedContact)
                message.IsViewed = false;
            else
                message.IsViewed = true;

            if(Contacts.FirstOrDefault(c=> c == message.Sender)==null)
                Contacts.Add(message.Sender);

            message.Sender.Messages.Add(message);

            message.Sender.OnPropertyChanged("UnreadCount");

        }

        public MainViewModel()
        {
            ///Init Chatbot
            chatBotOnBackground = new RandomChatBot(new(),3500);
            chatBotOnBackground.StartInParallel((Action<MessageModel>)RecieveMessage);
            /// init User
            Me = new ContactModel { Name = "OLEM", ImageSource = "https://sm.ign.com/ign_ap/cover/a/avatar-gen/avatar-generations_hugw.jpg" };

            /// Init "Online contacts"
            Contacts = new ObservableCollection<ContactModel>();
            for (int i = 0; i < 10; i++)
            {
                Contacts.Add(new ContactModel { Name = $"Contact {i}", ImageSource = "https://desu.shikimori.one/uploads/poster/characters/80711/main-8296f3421384e03e01e1635fa30ba906.webp" });
            }



            SendMessageCommand = new RelayCommand(o =>
            {
                /// Very brute type of check for content 
                var WithoutMarkup = TypedMessage.RemoveXMLMarkup();
                 if ( SelectedContact==null || string.IsNullOrWhiteSpace(WithoutMarkup))
                    return;

                Messages.Add(new MessageModel { IsViewed = false, External=false, Sender = Me, WhenSended = DateTime.Now, Message = TypedMessage });
                RandomChatBot.AddToQue(SelectedContact);
                TypedMessage = string.Empty;
            });

        }

        

    }

    
}
