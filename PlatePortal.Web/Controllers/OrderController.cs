using ClosedXML.Excel;
using GemBox.Document;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlatePortal.Domain.DomainModels;
using PlatePortal.Domain.DTO;
using PlatePortal.Domain.Identity;
using PlatePortal.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PlatePortal.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<PlatePortalApplicationUser> _userManager;


        public OrderController(IOrderService orderService, UserManager<PlatePortalApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;

            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }

        public IActionResult Index()
        {
            var result = this._orderService.getAllOrders();

            return View(result);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            BaseEntity model = new BaseEntity
            {
                Id = id
            };
            var result = this._orderService.getOrderDetails(model);
            var totalPrice = 0;
            ICollection<ProductInOrder> productInOrderList = result.ProductInOrders;
            foreach (var item in result.ProductInOrders)
            {
                totalPrice += item.Quantity * (int)item.SelectedProduct.MenuItemPrice;
            }
            OrderDto orderDto = new OrderDto();
            orderDto.ProductInOrders = productInOrderList;
            orderDto.TotalPrice = totalPrice;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            PlatePortalApplicationUser user = await _userManager.FindByIdAsync(userId);
            orderDto.User = user;
            return View(orderDto);


        }

        public FileContentResult CreateInvoice(Guid id)
        {
            BaseEntity model = new BaseEntity
            {
                Id = id
            };
            var result = this._orderService.getOrderDetails(model);

            var templatepath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatepath);

            document.Content.Replace("{{OrderNumber}}", result.Id.ToString());
            document.Content.Replace("{{CustomerEmail}}", result.User.Email);
            document.Content.Replace("{{CustomerAddress}}", result.User.Address);

            StringBuilder sb = new StringBuilder();
            var totalPrice = 0.0;

            foreach (var item in result.ProductInOrders)
            {
                totalPrice += item.Quantity * item.SelectedProduct.MenuItemPrice;
                sb.AppendLine(item.SelectedProduct.MenuItemName + " with quantity of: "
                    + item.Quantity + ", each with price of: "
                    + item.SelectedProduct.MenuItemPrice
                    + "den (from " + item.SelectedProduct.RestaurantName
                    + ", address: " + item.SelectedProduct.RestaurantAddress + ")");
            }
            document.Content.Replace("{{ProductsList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString() + "den");

            var stream = new MemoryStream();
            document.Save(stream, new PdfSaveOptions());
            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");

        }


        [HttpGet]
        public FileContentResult ExportAllOrders()
        {
            string fileName = "Orders.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Orders");

                worksheet.Cell(1, 1).Value = "Order Id";
                worksheet.Cell(1, 2).Value = "Customer Email";

                var result = this._orderService.getAllOrders();

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.User.Email;

                    for (int t = 1; t <= item.ProductInOrders.Count(); t++)
                    {
                        worksheet.Cell(1, t + 2).Value = "Product-" + (t);
                        worksheet.Cell(i + 1, t + 2).Value = item.ProductInOrders.ElementAt(t - 1).SelectedProduct.MenuItemName + " with price of " + item.ProductInOrders.ElementAt(t - 1).SelectedProduct.MenuItemPrice +"den";
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }


        }



    }
}
