using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SoruCevapPortali.Models;

namespace SoruCevapPortali.Hubs
{

    
    public class TodoHub : Hub
    {
        private readonly AppDbContext _context;

        public TodoHub(AppDbContext context)
        {
            _context = context;
        }
        public async Task SendMessage(string name)
        {
            var todo = new Todo
            {
                Name = name,
                IsActive = true 
            };

            // Veritabanına ekle ve kaydet
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", name);


        }
        public async Task DeleteTask(string name)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Name == name);
            if (todo != null)
            {
                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();
                await Clients.All.SendAsync("TaskDeleted", name); // Silinen veriyi tüm istemcilere bildirebilirsiniz.
            }
        }
    }
}