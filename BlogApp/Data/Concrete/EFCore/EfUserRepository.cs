using BlogApp.Data.Abstract;
using BlogApp.Entity;
using DataAccessLayer.Concrete;
using Microsoft.EntityFrameworkCore;

public class EfUserRepository : IUserRepository
{
    private readonly Context _context;
    public EfUserRepository(Context context)
    {
        _context = context;
    }

    public DbSet<User> Users => _context.Users;

    public async Task<User> GetById(int id)
    {
        try
        {
            var users = await _context.Users.ToListAsync();
            Console.WriteLine($"Toplam kullanıcı sayısı: {users.Count}");

            foreach (var u in users)
            {
                Console.WriteLine($"User ID: {u.UserId}, Name: {u.Name}");
            }

            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetById hata: {ex.Message}");
            throw;
        }
    }

    public void CreateUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void DeleteUser(User user)
    {
        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    public async Task UpdateUser(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}