using EduApp.Models;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain;

namespace EduApp.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<QuestionAttempt> QuestionAttempts { get; set; }


        // Reference identity user if needed
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<QuizAttempt>().HasKey(q => q.Id);

            modelBuilder.Entity<Question>()
                        .HasOne(q => q.Quiz)
                        .WithMany(qz => qz.Questions)
                        .HasForeignKey(q => q.QuizId)
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
