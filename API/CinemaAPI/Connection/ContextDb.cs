using CinemaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Connection
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<AgeRating> AgeRatings { get; set; }
        public DbSet<BarCategory> BarCategories { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<BarItem> BarItems { get; set; }
        public DbSet<BarOrder> BarOrders { get; set; }
        public DbSet<BarOrderItem> BarOrderItems { get; set; }
        public DbSet<Promo> Promos { get; set; }
    }
}
