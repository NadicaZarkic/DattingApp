using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitofWork
    {
        IUserRepository UserRepository { get; }
        IMessageRepository messageRepository { get; }
        ILikesRepository LikesRepository {get;}
        Task <bool> Complete();

        bool HasChanges();
    }
}