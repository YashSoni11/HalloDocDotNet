using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ViewModels
{
    public class ChatModel
    {
      
            public int ChatId { get; set; }

            public int AdminId { get; set; }

            public int ProviderId { get; set; }

            public int RequestId { get; set; }

            public int RoleId { get; set; }

            public string? Message { get; set; }

            public string? ChatDate { get; set; }

            public int SentBy { get; set; }

            public string? ChatBoxClass { get; set; }

            public string? RecieverName { get; set; }

            public string? flag { get; set; }

            public int GroupFlag { get; set; }
            public int Reciever1 { get; set; }
            public int Receiver2 { get; set; }

            public List<ChatModel> Chats { get; set; }
        
    }
}
