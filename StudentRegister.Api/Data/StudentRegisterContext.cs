using Microsoft.EntityFrameworkCore;
using StudentRegister.Api.Models;

namespace StudentRegister.Api.Data
{
    public class StudentRegisterContext(DbContextOptions<StudentRegisterContext> options) : DbContext(options)
    {
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.StudentNumber).IsUnique();
                entity.Property(x => x.FirstName).HasMaxLength(100).IsRequired();

                entity.HasData(
                     new() { Id = 1, FirstName = "Thabo", LastName = "Mokoena", StudentNumber = "ST001", Gender = "M" },
                    new() { Id = 2, FirstName = "Naledi", LastName = "Dlamini", StudentNumber = "ST002", Gender = "F" },
                    new() { Id = 3, FirstName = "Sipho", LastName = "Ndlovu", StudentNumber = "ST003", Gender = "M" },
                    new() { Id = 4, FirstName = "Tracy", LastName = "Jones", StudentNumber = "ST004", Gender = "F" }
                    );
            });

        }
    }
}
