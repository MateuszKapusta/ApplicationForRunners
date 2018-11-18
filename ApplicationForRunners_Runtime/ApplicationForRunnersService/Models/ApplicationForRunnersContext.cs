using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.Azure.Mobile.Server.Tables;
using ApplicationForRunnersService.DataObjects;

namespace ApplicationForRunnersService.Models
{
    public class ApplicationForRunnersContext : DbContext
    {
        // If you want Entity Framework to alter your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        private const string connectionStringName = "Name=MS_TableConnectionString";

        public ApplicationForRunnersContext() : base(connectionStringName)
        {
        } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
        }

        public DbSet<WorkoutItem> WorkoutItems { get; set; }
        public DbSet<UserItem> UserItems { get; set; }
        public DbSet<MapPointItem> MapPointItems { get; set; }
    }
}
