using System.Data.Entity;

namespace DepartmentStructure
{
    public partial class Context : DbContext
    {
        public Context()
            : base("Connect")
        { }

        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Empoyee> Empoyee { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>()
                .HasMany(e => e.NestedDepartments)
                .WithOptional(e => e.ParentDepartment)
                .HasForeignKey(e => e.ParentDepartmentID);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Empoyee)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Empoyee>()
                .Property(e => e.ID)
                .HasPrecision(5, 0);
        }
    }
}
