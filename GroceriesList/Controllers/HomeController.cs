using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GroceriesList.Entities;
using GroceriesList.Helpers;
using GroceriesList.Models;

namespace GroceriesList.Controllers {
    public class HomeController : Controller {
        private readonly IGroceryListRepository _data;

        public HomeController(IGroceryListRepository data) {
            _data = data;
        }

        public ActionResult Index() {
            var mobileDevice = Request.Browser.Browser.ToLower();
            Debug.WriteLine(mobileDevice);
            switch (mobileDevice) {
                case "ie":
                case "iemobile":
                    return View("metro");
                default:
                    return View("ios");
            }
               
        }

        public JsonResult Groceries() {
            return Json(GetGroceries(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Products() {
            return Json(GetDividerProducts(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddProduct(string product, bool addToList) {
            var result = new AddProductResult();

            var productEncoded = Server.HtmlEncode(product);
            if (!string.IsNullOrWhiteSpace(product)) {
                var currentProduct = _data.GetProducts().FirstOrDefault(p => p.Name.ToUpper() == product.ToUpper());
                if (currentProduct != null) {
                    result.Message = string.Format("Product  '{0}'  already exists.", productEncoded);
                } else {
                    var newProduct = _data.AddProduct(new Product() { Name = product.ToFirstLetterCapitalized() });
                    if (addToList) {
                        _data.AddGrocery(new Entities.Grocery() { DateCreated = DateTime.Now, ProductId = newProduct.Id });
                    }
                    _data.SaveChanges();
                    result.Id = newProduct.Id;
                    result.Name = newProduct.Name;
                    result.Message = string.Format("Added product  '{0}'.", productEncoded);
                }
            } else {
                result.Message = "No product entered!";
            }

            return Json(result);
        }

        public ActionResult AddGrocery(int id) {
            if (_data.GetGroceries().FirstOrDefault(p => p.ProductId == id) == null) {
                _data.AddGrocery(new Entities.Grocery() { DateCreated = DateTime.Now, ProductId = id });
                _data.SaveChanges();
            }

            return new EmptyResult();
        }

        public ActionResult RemoveGrocery(int id) {
            var itemToRemove = _data.GetGrocery(id);
            if (itemToRemove != null) {
                _data.RemoveGrocery(itemToRemove);
                _data.SaveChanges();
            }

            return new EmptyResult();
        }

        public ActionResult RemoveProduct(int id) {
            var product = _data.GetProduct(id);
            if (product != null) {
                _data.RemoveProduct(product);
                _data.SaveChanges();
            }

            return new EmptyResult();
        }

        public ActionResult ClearGroceries() {
            var list = _data.GetGroceries().ToList();
            foreach (var item in list) {
                _data.RemoveGrocery(item);
            }
            _data.SaveChanges();
            return new EmptyResult();
        }

        private void IsAjax() {
            if (Request.IsAjaxRequest()) {
                Debug.WriteLine("Ajax Request");
            } else {
                Debug.WriteLine("Not Ajax Request");
            }
        }

        private List<Product> GetDividerProducts() {
            List<Product> selectedProducts = null;
            if (!_data.GetGroceries().Any()) {
                selectedProducts = _data.GetProducts().ToList();
            } else {

                var groceries = _data.GetGroceries().Select(p => p.ProductId).ToArray();
                var products = _data.GetProducts().Select(p => p.Id).ToArray();
                var unselected = products.Except(groceries);

                selectedProducts = _data.GetProducts().Where(p => unselected.Contains(p.Id)).ToList();
            }

            var alphabet = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            var dividerProducts = new List<Product>();
            var orderProductList = selectedProducts.OrderBy(o => o.Name).ToList();

            foreach (var product in orderProductList) {
                var letter = product.Name.Substring(0, 1).ToLower();
                if (alphabet.Contains(letter)) {
                    dividerProducts.Add(new Product() { Id = -1, Name = letter });
                    alphabet.Remove(letter);
                }
                dividerProducts.Add(product);
            }
            return dividerProducts;
        }

        private List<GroceryViewModel> GetGroceries() {
            var groceryList = _data.GetGroceries().OrderBy(o => o.Product.Name)
                .Select(p => new GroceryViewModel() { ProductId = p.ProductId, ProductName = p.Product.Name })
                .ToList();
            return groceryList;
        }
    }
}
