namespace StoreProject.Models
{
    public class OrdersHistory
    {
        public int TxNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string ActionOrder { get; set; }
        public string StatusOrder { get; set; }
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
