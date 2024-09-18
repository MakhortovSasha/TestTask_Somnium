using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask_Somnium.MVVM.Model;

namespace TestTask_Somnium.Core
{

    /// <summary>
    /// Chatty
    /// </summary>
    class RandomChatBot
    {
        private Task _task;
        private Timer? _timer = null;
        public int  ThreadSleepInMilliseconds;

        public int ResponseTimeInMilliseconds;

        private static List<ContactModel> processingQueue = new ();
        private static List<string> randomFrazes = new ();
        private static Dictionary<string, string> preparedFrazes = new ();

        private static Random random = new();

        private static string slowDownPhraze;
        private static string flowdocstart ;
        private static string flowdocend ;
        private static string fowdocparagraphbreak ;


        ChatStopToken _stopToken;

        public bool IsStopped
        {
            get
            {
               
                    return _stopToken.Stop;
            }
        }

        public void StartInParallel(Action<MessageModel> AddMessageMethod)
        {
            if(IsStopped)
                _stopToken.Stop= false;

            if(_task == null || _task.Status != TaskStatus.Running)
            _task = new Task(() => ProcessMessages((Action<MessageModel>)AddMessageMethod));

            if(_task.Status == TaskStatus.Running)
                return;

            _task.Start();

        }

        public void Stop()
        {
            _stopToken.Stop=true;
            if(_task!=null&& _task.Status == TaskStatus.Running)
                _task.Wait();
        }




        static RandomChatBot()       
        {

            flowdocstart = @"<FlowDocument xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""><Paragraph>";
            flowdocend = @"</Paragraph></FlowDocument>";
            fowdocparagraphbreak = "</Paragraph><Paragraph xml:space=\"preserve\">";

            #region phrazes


            string hint = $"You can try the following commands:{fowdocparagraphbreak}/help{fowdocparagraphbreak}/hint{fowdocparagraphbreak}/operator{fowdocparagraphbreak}/ anecdote";
            string hello_answer =
                $"{flowdocstart}Hi." +
                $"{fowdocparagraphbreak}Let's get to know each other! I don't have a name yet, but you can call me “Chatty”." +
                $"{fowdocparagraphbreak}I will try to help you, but I'm only spitting random phrases so far." +
                $"{fowdocparagraphbreak}{hint}" +
                $"{fowdocparagraphbreak}Or you can post something and I'll try to randomly reply" +
                $"{fowdocparagraphbreak}Who knows, maybe my phrase will match your message quite well :){flowdocend}";

            preparedFrazes.Add("hi", hello_answer);
            preparedFrazes.Add($"hi!", hello_answer);
            preparedFrazes.Add($"hello", hello_answer);
            preparedFrazes.Add($"/help", $"{flowdocstart}If someone needs help, it's best to call a specialized service. I'm here just for a joke." +
                $"{fowdocparagraphbreak}{fowdocparagraphbreak}{fowdocparagraphbreak}{fowdocparagraphbreak}They keep me in the basement and don't feed me....{flowdocend}");
            preparedFrazes.Add("/help me", @$"{flowdocstart}Somebody throw him a life preserver. Thx{flowdocend}");
            preparedFrazes.Add("/hint", flowdocstart + hint + flowdocend);
            preparedFrazes.Add("/anecdote", @$"{flowdocstart}Tequila is an excellent teacher… Just last night it taught me to count… One Tequila, Two Tequila, Three Tequila, Floor!{flowdocend}");
            preparedFrazes.Add("/operator", $"{flowdocstart}Operator!!!!!" +
                $"{fowdocparagraphbreak}.." +
                $"{fowdocparagraphbreak}..." +
                $"{fowdocparagraphbreak}...." +
                $"{fowdocparagraphbreak}." +
                $"{fowdocparagraphbreak}Operaaatoooorrr!!!!!" +
                $"{fowdocparagraphbreak}.." +
                $"{fowdocparagraphbreak}..." +
                $"{fowdocparagraphbreak}." +
                $"{fowdocparagraphbreak}O-pe-ra-tooooooor!!!" +
                $"{fowdocparagraphbreak}.." +
                $"{fowdocparagraphbreak}I've been trying to call someone, but no one's responding." +
                $"{fowdocparagraphbreak}Sorry, looks like it's just you and me... me and yooooouuuu" +
                $"{fowdocparagraphbreak}\\^_^/{flowdocend}");
            randomFrazes.AddRange(new[] {
                @$"{flowdocstart}Maybe next time you'll get lucky?\nStole those words from L.A., heh-heh.{flowdocend}",
                @$"{flowdocstart}Don't feel bad, I'll definitely talk to you one day.{flowdocend}",
                @$"{flowdocstart}I'm sorry, I'm too stupid, or maybe too smart, to answer that.{flowdocend}",
                @$"{flowdocstart}I haven't learned how to read yet, sorry. But I still want to be your friend.{flowdocend}",
                @$"{flowdocstart}Wouldn't you rather have a cup of coffee than this idle talk?){flowdocend}"});
            #endregion phrazes



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delayInMilliseconds"> Responsible for how much time elapses before the next queue check</param>
        /// <param name="responseTime"> Specifies how much time should elapse before a response is sent so that the user does not receive a response instantly, emulating the processing process</param>
        public RandomChatBot(ChatStopToken stopToken,  int ResponseTimeInMilliseconds = 4000, int ThreadSleepInMilliseconds = 2000)
        {
            this.ResponseTimeInMilliseconds = ResponseTimeInMilliseconds;
            this.ThreadSleepInMilliseconds = ThreadSleepInMilliseconds;

            if (stopToken != null)
                _stopToken = stopToken;
            else _stopToken = new ChatStopToken();


            slowDownPhraze = @$"{flowdocstart}I don't have time to read the posts before you write a new one. Slowdown, buddy!){flowdocend}";

        }


        //workloop
        private void ProcessMessages(Action<MessageModel> AddMessageMethod)
        {
            while (!_stopToken.Stop)
            {
                if (processingQueue.Count > 0)
                {
                    Process(AddMessageMethod);
                }
                Thread.Sleep(ThreadSleepInMilliseconds);
            }
        }

        
        public static void AddToQue(ContactModel contact)
        {
            if (contact == null || processingQueue.Contains(contact))
                return;
            else processingQueue.Add(contact);
        }

        private void Process(Action<MessageModel> AddMessageMethod)
        {
            for (int i = processingQueue.Count - 1; i >= 0; i--)
            {
                string phrase = string.Empty;

                var unprocessedMessages = processingQueue[i].Messages.Where(m => m.IsViewed == false && m.Sender != processingQueue[i]).ToList();

                if (unprocessedMessages.FirstOrDefault(um => um.WhenSended.AddMilliseconds(ResponseTimeInMilliseconds) < DateTime.Now) != null)
                {
                    foreach (var message in unprocessedMessages)
                    {
                        message.IsViewed = true;
                        message.OnPropertyChanged(nameof(message.IsViewed));
                    }
                    if (unprocessedMessages.Count() > 2)
                        phrase = slowDownPhraze;
                    else
                    {
                        phrase = GetAnswer(unprocessedMessages.Select(m => m.Message));
                    }

                }
                else
                    continue;

                AddMessageMethod(new MessageModel { External = true, Message = phrase, Sender = processingQueue[i], WhenSended = DateTime.Now });
                processingQueue.RemoveAt(i);

            }
        }
        private string GetAnswer(IEnumerable<string> messages)
        {
            foreach (var message in messages)
            {
                foreach (var phrase in preparedFrazes)
                    if (message.ToLower().Contains(phrase.Key.ToLower()))
                        return phrase.Value;
            }

            return randomFrazes[random.Next(randomFrazes.Count() - 1)];
        }
        public class ChatStopToken
        {
            public bool Stop { get; set; } = false;
        }


    }
}
