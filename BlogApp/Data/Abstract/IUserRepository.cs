using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlogApp.Entity;

namespace BlogApp.Data.Abstract
{
    public interface IUserRepository
    {
        DbSet<User> Users { get; }
        void CreateUser(User user);
        void DeleteUser(User user);
        Task<User> GetById(int id);
        Task UpdateUser(User user);
        Task SaveAsync();
    }
}