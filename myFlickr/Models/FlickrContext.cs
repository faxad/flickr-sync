using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace myFlickr.Model
{
    class FlickrContext : DbContext
    {
        public DbSet<FlickrInfo> Downloads { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();
        }
    }
}
