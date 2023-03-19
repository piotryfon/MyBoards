using Microsoft.EntityFrameworkCore;

namespace MyBoards.Entities
{
    public class MyBoardsContext : DbContext
    {
        public MyBoardsContext(DbContextOptions<MyBoardsContext>options) : base(options)
        {

        }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;DataBase=MyBoardsDb;Trusted_Connection=True");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //tworzenie złożonego klucza
            //modelBuilder.Entity<User>()
            //     .HasKey(x => new { x.Email, x.LastName });

            modelBuilder.Entity<WorkItem>()
                .Property(x => x.State).IsRequired();

            modelBuilder.Entity<WorkItem>(builder =>
            {
                builder.Property(wi => wi.IterationPath).HasColumnName("Iteration_Path");
                builder.Property(wi => wi.Area).HasColumnType("varchar(200)");
                builder.Property(wi => wi.Efford).HasColumnType("decimal(5,2)");
                builder.Property(wi => wi.EndDate).HasPrecision(3); 
                builder.Property(wi => wi.Activity).HasMaxLength(200);
                builder.Property(wi => wi.RemaningWork).HasPrecision(14,2);
                builder.Property(wi => wi.Priority).HasDefaultValue(1);
                // workitem ma wiele kometarzy
                builder.HasMany(prop => prop.Comments).WithOne(comment => comment.WorkItem).HasForeignKey(comment => comment.WorkItemId);
                builder.HasOne(w => w.Author).WithMany(u => u.WorkItems).HasForeignKey(w => w.AuthorId);
            });

            modelBuilder.Entity<Comment>(builder =>
            {
                builder.Property(x => x.CreatedDate).HasDefaultValueSql("getutcdate()");
                builder.Property(x => x.UpdatedDate).ValueGeneratedOnUpdate();  
            });


            modelBuilder.Entity<User>().HasOne(u => u.Address).WithOne(u => u.User).HasForeignKey<Address>(a=>a.UserId);
          
        }
    }
}
