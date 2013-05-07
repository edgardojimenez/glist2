using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroceriesList.Models;

namespace GroceriesList.Entities {
    public class GroceryListEfRepository : IGroceryListRepository {
        private DataContext _data;

        public GroceryListEfRepository(DataContext data) {
            _data = data;
        }

        public IEnumerable<Grocery> GetGroceries() {
            return _data.Groceries.ToList();
        }

        public IEnumerable<Product> GetProducts() {
            return _data.Products.ToList();
        }

        public Product GetProduct(string name) {
            return _data.Products.SingleOrDefault(p => p.Name == name);
        }

        public Product GetProduct(int id) {
            return _data.Products.SingleOrDefault(p => p.Id == id);
        }

        public Product AddProduct(Product product) {
            return _data.Products.Add(product);
        }


        public Grocery AddGrocery(Grocery grocery) {
            return _data.Groceries.Add(grocery);
        }

        public Grocery GetGrocery(int id) {
            return _data.Groceries.SingleOrDefault(p => p.ProductId == id);
        }

        public void RemoveGrocery(Grocery grocery) {
            _data.Groceries.Remove(grocery);
        }

        public void RemoveProduct(Product product) {
            _data.Products.Remove(product);
        }


        public int SaveChanges() {
            return _data.SaveChanges();
        }
    }
}