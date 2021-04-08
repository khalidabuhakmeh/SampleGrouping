using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace SampleGrouping.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<Override> Overrides { get; set; }
        public DbSet<BudgetUser> BudgetUsers { get; set; }
    }

    public class Override
    {
        public int Id { get; set; }
        public string BudgetYear { get; set; }
        [Required(AllowEmptyStrings = false)] public string PropertyId { get; set; }
        public string BudgetId { get; set; }
        [Required(AllowEmptyStrings = false)] public string Account { get; set; }
        public DateTime Bymmddyyyy { get; set; }
        public float? OverrideAmount { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }
        public int? BudgetUserId { get; set; }
        public virtual BudgetUser User { get; set; }
    }

    public class BudgetUser
    {
        public int Id { get; set; }
        public string BudgetUsername { get; set; }
        public string Email { get; set; }
    }
}