using Microsoft.AspNetCore.Mvc;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RevisioneNew.Interfaces;
using RevisioneNew.Models;
using RevisioneNew.Services;

namespace RevisioneNew.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderInterface _orderService;
        public OrderController(IOrderInterface oredrService) 
        { 
            _orderService = oredrService;
        }

        [HttpPost]
        public JsonResult LoadOrderList()
        {
            ViewBag.Order = "active";
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                PagingInfo pagingInfo = new PagingInfo()
                {
                    Count = pageSize,
                    PageNumber = (skip / pageSize) + 1,
                    ReturnTotalRecordCount = true
                };
                Datatable<OrderModel> orders = _orderService.GetaAllOrders(draw, sortColumn, pagingInfo, searchValue, sortColumnDirection);

                return Json(orders);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Fulfill Order
        [HttpPost]
        public IActionResult FulfillOrder(string orderId)
        {
            try
            {
                if (orderId != null)
                {
                    Guid fulfillOrderId = new Guid(orderId);
                    bool result = _orderService.FulfillOrder(fulfillOrderId);
                    if(result)
                    {
                        return Ok("Order is successfully fulfilled.");
                    }
                    else
                    {
                        return BadRequest("There is some error occured.");
                    }
                }

                return BadRequest("Order not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Cancel Order
        [HttpPost]
        public IActionResult CancelOrder(string orderId)
        {
            try
            {
                if (orderId != null)
                {
                    Guid cancelOrderId = new Guid(orderId);
                    bool result = _orderService.CancelOrder(cancelOrderId);
                    if (result)
                    {
                        return Ok("Order is successfully canceled.");
                    }
                    else
                    {
                        return BadRequest("Ther is some error occured during creation of error");
                    }
                }

                return BadRequest("Order not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Create Invoice from Order
        [HttpPost]
        public IActionResult CreateInvoice(string orderId)
        {
            try
            {
                if (!string.IsNullOrEmpty(orderId))
                {
                    Guid invoiceOrderId = new Guid(orderId);
                    string result = _orderService.CreateInvoice(invoiceOrderId);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return Ok("Invoice created with invoice number : "+result);
                    }
                    else
                    {
                        return BadRequest("Error during error creation.");
                    }
                }

                return BadRequest("Order not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult DeleteOrder(string orderId)
        {
            try
            {
                if (!string.IsNullOrEmpty(orderId))
                {
                    Guid orderGuid = new Guid(orderId);
                    _orderService.DeleteOrder(orderGuid);
                    return Ok("Order deleted successfully.");
                }
                return BadRequest("Order not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            }


        [Route("/Orders")]
        public IActionResult Orders()
        {
            ViewBag.Order = "active";
            return View();
        }
    }
}
