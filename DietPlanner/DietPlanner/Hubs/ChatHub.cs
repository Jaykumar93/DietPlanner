using Microsoft.AspNetCore.SignalR;
using Repository.Interfaces;


namespace Web.Hubs
{
   
    public class ChatHub : Hub
    {
        private readonly IUserPostRepository _userPostRepository;

        public ChatHub(IUserPostRepository userPostRepository)
        {
            _userPostRepository = userPostRepository;
        }
        public override Task OnConnectedAsync()
        {
            Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            return base.OnConnectedAsync();
        }
        public async Task SendMessage(string user, string message)
        {
            //message send to all users
            _userPostRepository.CreateNewMessage( user,  message);

            await Clients.All.SendAsync("ReceiveMessage", user, message);

        }

    }
}
