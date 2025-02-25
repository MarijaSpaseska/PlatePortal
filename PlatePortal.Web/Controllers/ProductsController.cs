using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlatePortal.Domain.DomainModels;
using PlatePortal.Domain.DTO;
using PlatePortal.Services.Interface;

namespace PlatePortal.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        //ImportProductsPage
        public IActionResult ImportProductsPage()
        {
            var allProducts = this._productService.GetAllProducts();
            return View(allProducts);
        }



        //ImportProducts
        public IActionResult ImportProducts(IFormFile file)
        {
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }

            List<ProductDto> products = getAllProductsFromFile(file.FileName);

            foreach (var item in products)
            {
                //var userCheck = userManager.FindByEmailAsync(item.Email).Result;
                var productCheck = _productService.GetAllProducts().FirstOrDefault(z => z.MenuItemDescription == item.MenuItemDescription);
                if (productCheck == null)
                {
                    var product = new Product
                    {
                        MenuItemName = item.MenuItemName,
                        MenuItemDescription = item.MenuItemDescription,
                        MenuItemImage = item.MenuItemImage,
                        MenuItemPrice = item.MenuItemPrice,
                        MenuItemType = item.MenuItemType,
                        RestaurantName = item.RestaurantName,
                        RestaurantAddress = item.RestaurantAddress,
                        RestaurantCuisine = item.RestaurantCuisine,
                        RestaurantDeliveryTime = item.RestaurantDeliveryTime,
                        RestaurantLogo = item.RestaurantLogo,
                        RestaurantOpeningHours=item.RestaurantOpeningHours
                    };
                    _productService.CreateNewProduct(product);

                }
                else
                {
                    continue;
                }
            }
            return RedirectToAction("Index", "Products");
        }


        private List<ProductDto> getAllProductsFromFile(string fileName)
        {
            List<ProductDto> products = new List<ProductDto>();
            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        products.Add(new ProductDto
                        {
                            MenuItemName = reader.GetValue(0).ToString(),
                            MenuItemPrice = (double)reader.GetValue(1),
                            MenuItemImage = reader.GetValue(2).ToString(),
                            MenuItemDescription = reader.GetValue(3).ToString(),
                            RestaurantName = reader.GetValue(4).ToString(),
                            RestaurantAddress = reader.GetValue(5).ToString(),
                            RestaurantOpeningHours = reader.GetValue(6).ToString(),
                            RestaurantDeliveryTime = reader.GetValue(7).ToString(),
                            RestaurantCuisine = reader.GetValue(8).ToString(),
                            MenuItemType = reader.GetValue(9).ToString(),
                            RestaurantLogo = reader.GetValue(10).ToString()
                        });
                    }
                }
            }
            return products;
        }

        [HttpPost]
        public bool ImportAllProducts(List<ProductDto> model)
        {
            bool status = true;
            foreach (var item in model)
            {
                var productCheck = _productService.GetAllProducts().FirstOrDefault(z => z.MenuItemDescription == item.MenuItemDescription);
                if (productCheck == null)
                {
                    var product = new Product
                    {
                        //Id = Guid.NewGuid(),
                        MenuItemName = item.MenuItemName,
                        MenuItemDescription = item.MenuItemDescription,
                        MenuItemImage = item.MenuItemImage,
                        MenuItemPrice = item.MenuItemPrice,
                        MenuItemType = item.MenuItemType,
                        RestaurantName = item.RestaurantName,
                        RestaurantAddress = item.RestaurantAddress,
                        RestaurantCuisine = item.RestaurantCuisine,
                        RestaurantDeliveryTime = item.RestaurantDeliveryTime,
                        RestaurantLogo = item.RestaurantLogo,
                        RestaurantOpeningHours = item.RestaurantOpeningHours
                    };
                    _productService.CreateNewProduct(product);

                    
                }
                else
                {
                    continue;
                }
            }

            return status;
        }



        // GET: AddProductToCart
        public IActionResult AddProductToCart(Guid? id)
        {
            var model = this._productService.GetShoppingCartInfo(id);
            return View(model);
        }

        // POST: AddProductToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProductToCart([Bind("ProductId,Quantity")] AddToShoppingCartDto item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = this._productService.AddToShoppingCart(item, userId);
            if (result)
            {
                return RedirectToAction("Index", "Products");
            }
            return View(item);

        }

        // GET: Products
        public IActionResult Index()
        {
            var allProducts = this._productService.GetAllProducts();
            return View(allProducts);
        }

        // GET: Products - Appetizers
        public IActionResult Appetizers()
        {
            var allProducts = this._productService.GetAllProducts().FindAll(z => z.MenuItemType == "Appetizer");
            return View(allProducts);
        }

        // GET: Products - MainCourses
        public IActionResult MainCourses()
        {
            var allProducts = this._productService.GetAllProducts().FindAll(z => z.MenuItemType == "Main course");
            return View(allProducts);
        }

        // GET: Products - Desserts
        public IActionResult Desserts()
        {
            var allProducts = this._productService.GetAllProducts().FindAll(z => z.MenuItemType == "Dessert");
            return View(allProducts);
        }

        // GET: Products/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = this._productService.GetDetailsForProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,MenuItemName,MenuItemType,MenuItemDescription,MenuItemPrice,MenuItemImage,RestaurantName,RestaurantAddress,RestaurantOpeningHours,RestaurantDeliveryTime,RestaurantCuisine,RestaurantLogo")] Product product)
        {
            if (ModelState.IsValid)
            {
                this._productService.CreateNewProduct(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = this._productService.GetDetailsForProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,MenuItemName,MenuItemType,MenuItemDescription,MenuItemPrice,MenuItemImage,RestaurantName,RestaurantAddress,RestaurantOpeningHours,RestaurantDeliveryTime,RestaurantCuisine,RestaurantLogo")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    this._productService.UpdeteExistingProduct(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = this._productService.GetDetailsForProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            this._productService.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
            return this._productService.GetDetailsForProduct(id) != null;
        }
    }
}
