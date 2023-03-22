using Microsoft.EntityFrameworkCore;

namespace MyBoards.Entities
{
    public class MyBoardsContext : DbContext
    {
        public MyBoardsContext(DbContextOptions<MyBoardsContext>options) : base(options)
        {

        }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Epic> Epics { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<WorkItemState> States { get; set; }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;DataBase=MyBoardsDb;Trusted_Connection=True");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //tworzenie złożonego klucza
            //modelBuilder.Entity<User>()
            //     .HasKey(x => new { x.Email, x.LastName });

            //modelBuilder.Entity<WorkItem>()
            //    .Property(x => x.State).IsRequired();

            modelBuilder.Entity<Epic>().Property(wi => wi.EndDate).HasPrecision(3);

            modelBuilder.Entity<Issue>().Property(wi => wi.Efford).HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Task>()
                .Property(wi => wi.Activity).HasMaxLength(200)
                .HasPrecision(14, 2);
            

            modelBuilder.Entity<WorkItemState>().Property(s => s.Value).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<WorkItem>(builder =>
            {
                builder.HasOne(w => w.State).WithMany().HasForeignKey(w => w.StateId);

                builder.Property(wi => wi.IterationPath).HasColumnName("Iteration_Path");
                builder.Property(wi => wi.Area).HasColumnType("varchar(200)");
                builder.Property(wi => wi.Priority).HasDefaultValue(1);
                // workitem ma wiele kometarzy
                builder.HasMany(prop => prop.Comments).WithOne(comment => comment.WorkItem).HasForeignKey(comment => comment.WorkItemId);
                builder.HasOne(w => w.Author).WithMany(u => u.WorkItems).HasForeignKey(w => w.AuthorId);
                // wiele do wielu
                builder.HasMany(workItem => workItem.Tags).WithMany(tag => tag.WorkItems);
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
