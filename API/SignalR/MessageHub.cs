using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub: Hub
    {  
        private readonly IMapper _mapper;
        private readonly IUnitofWork _uow;
       
        private readonly IHubContext<PresenceHub> __presenceHub;
        public MessageHub(IUnitofWork uow, IMapper mapper, IHubContext<PresenceHub> _presenceHub)
        {
            _uow = uow;
            __presenceHub = _presenceHub;
            _mapper = mapper;
          

           
        }

        public override async Task OnConnectedAsync()
        {
           var httpContext = Context.GetHttpContext();
           var otherUser = httpContext.Request.Query["user"];
           var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

           await Groups.AddToGroupAsync(Context.ConnectionId,groupName);

           var group = await AddtoGroup(groupName);

           await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
          
           var messages = await _uow.messageRepository
           .GetMessageThread(Context.User.GetUsername(),otherUser);

           if (_uow.HasChanges()) await _uow.Complete();

           await Clients.Caller.SendAsync("ReceiveMessageThread",messages);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
           var group =  await RemoveFromMessageGroup();
           await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();

            if(username == createMessageDto.RecipientUsername.ToLower())
            {
                throw new HubException("You cannot send messages to yourself");
            }

            var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);

            var recipient = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if(recipient == null) 
                throw new HubException("Not found user");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName,recipient.UserName);

            var group = await _uow.messageRepository.GetMessageGroup(groupName);

            if(group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connection = await PresenceTracker.GetConnectionsForUser(recipient.UserName);

                if(connection != null)
                {
                    await __presenceHub.Clients.Clients(connection).SendAsync("NewMessageReceived",
                    new {username = sender.UserName,knownAs = sender.KnownAs});
                }
            }

            _uow.messageRepository.AddMessage(message);

            if(await _uow.Complete()) 
            {
                
                await Clients.Group(groupName).SendAsync("NewMessage",_mapper.Map<MessageDto>(message));
            }
            

        }

        private string GetGroupName(string caller,string other)
        {
            var stringCompare = string.CompareOrdinal(caller,other) < 0;

            return stringCompare ? $"{caller}- {other}" : $"{other}-{caller}";
        }

        private async Task<Group> AddtoGroup(string groupName)
        {
            var group = await _uow.messageRepository.GetMessageGroup(groupName);

            var connection = new Connection(Context.ConnectionId,Context.User.GetUsername());

            if(group == null)
            {
                group = new Group(groupName);
                _uow.messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if (await _uow.Complete()) return group;

            throw new HubException("Failed to add to group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _uow.messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x=> x.ConnectionId == Context.ConnectionId);
            _uow.messageRepository.RemoveConnection(connection);

           if (await _uow.Complete()) return group;

           throw new HubException("Failed to remove from group");

        }
        
        
    }
}