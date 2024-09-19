using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreProject.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoreProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersHistoryController : ControllerBase
    {
        
        private readonly OrderHistoryRepository _repository;

        public OrdersHistoryController(OrderHistoryRepository repository)
        {
            _repository = repository;
        }

        // GET: api/OrdersHistory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersHistory>>> GetOrders()
        {
            try
            {
                var orders = _repository.GetOrders();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                // Manejo de Errores
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/OrdersHistory/completed
        [HttpGet("completed")]
        public async Task<ActionResult<IEnumerable<OrdersHistory>>> GetCompletedOrders()
        {
            try
            {
                var orders = _repository.GetCompletedOrders();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/OrdersHistory/year/2023
        [HttpGet("year/{year}")]
        public async Task<ActionResult<IEnumerable<OrdersHistory>>> GetOrdersByYear(int year)
        {
            try
            {
                var orders = _repository.GetOrdersByYear(year);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/OrdersHistory
        [HttpPost]
        public IActionResult AddOrder([FromBody] OrdersHistory newOrder)
        {
            if (newOrder == null)
            {
                return BadRequest("Order is null.");
            }

            if (string.IsNullOrEmpty(newOrder.ActionOrder) ||
                newOrder.Quantity <= 0 ||
                newOrder.Price <= 0)
            {
                return BadRequest("Invalid order data.");
            }

            try
            {
                _repository.AddOrder(newOrder);
                return CreatedAtAction(nameof(GetOrdersByYear), new { year = DateTime.Now.Year }, newOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{txNumber}")]
        public IActionResult UpdateOrderStatus(int txNumber, [FromBody] string newStatus)
        {
            if (newStatus != "EXECUTED" && newStatus != "CANCELED")
            {
                return BadRequest("El estado debe ser 'EXECUTED' o 'CANCELED'.");
            }

            var result = _repository.UpdateOrderStatus(txNumber, newStatus);
            if (result)
            {
                return NoContent(); // Estado actualizado
            }

            return NotFound(); // Orden no encontrada o no se pudo actualizar
        }





        //[HttpGet]
        //[Route("getOrdersHistory")]
        //public List<OrdersHistory> getOrdersHistory()
        //{

        //    List<OrdersHistory> lh = new List<OrdersHistory>();

        //    OrdersHistory oh = new OrdersHistory();

        //    return lh;
        //}

    }
}
