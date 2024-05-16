using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.SignalR;


namespace dotnetProc.Hubs
{
    public class ChatHub:Hub
    {

        private readonly HalloDocContext _context;
        private readonly IGenericRepository<Chat> _chatRepo;

        public ChatHub(HalloDocContext context, IGenericRepository<Chat> chatRepo)
        {
            _context = context;
            _chatRepo = chatRepo;
        }

        public async Task SendMessage(string message, string RequestID, string ProviderID, string AdminID, string RoleID, string GroupFlagID)
        {
            
            Chat chat = new Chat();
            chat.Message = message;
            chat.Sentby = Convert.ToInt32(RoleID);
            chat.Adminid = Convert.ToInt32(AdminID);
            chat.Requestid = Convert.ToInt32(RequestID);
            chat.Physicianid = Convert.ToInt32(ProviderID);
            chat.Sentdate = DateTime.Now;
            chat.Chattype = Convert.ToInt32(GroupFlagID);
            _chatRepo.Add(chat);
            

            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
