namespace StoreProject.Models
{
    public class Stock
    {
        public int ProductId { get; set; } // Mapea a PRODUCT_ID
        public string Symbol { get; set; }  // Mapea a SYMBOL
        public string ProductName { get; set; } // Mapea a PRODUCT_NAME
        public int CurrentStock { get; set; } // Mapea a CURRENT_STOCK
    }
}
