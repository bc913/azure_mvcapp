using System.Collections.Generic;
using System.Threading.Tasks;
using MvcApp.Models;

namespace MvcApp.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetAsync();
    }
}