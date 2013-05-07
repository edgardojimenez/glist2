using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GroceriesList.Entities {
    public partial class DataContext : DbContext {

        public const string ConnectionString = "name=GroceriesConnection";

        #region Constructors

        public DataContext()
            : base(ConnectionString) {
        }

        public DataContext(string connectionString)
            : base(connectionString) {
        }

        #endregion

        #region DbSet Properties

        public DbSet<Product> Products { get; set; }
        public DbSet<Grocery> Groceries { get; set; }
        
        #endregion
    }
}