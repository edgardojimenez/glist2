using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroceriesList.Models;

namespace GroceriesList.Entities {
    public interface IGroceryListRepository {
        IEnumerable<Grocery> GetGroceries();
        IEnumerable<Product> GetProducts();

        Product GetProduct(string name);
        Product GetProduct(int id);
        Product AddProduct(Product product);
        Grocery AddGrocery(Grocery grocery);
        Grocery GetGrocery(int id);
        void RemoveGrocery(Grocery grocery);
        void RemoveProduct(Product product);

        int SaveChanges();
    }
}