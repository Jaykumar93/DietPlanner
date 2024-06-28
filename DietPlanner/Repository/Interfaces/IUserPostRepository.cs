using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IUserPostRepository
    {
        public void CreateNewMessage(string userName, string userMessage);
    }
}
