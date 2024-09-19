using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;  // Importante para usar ADO.NET
using Microsoft.Extensions.Configuration;  // Para obtener la cadena de conexión
using StoreProject.Models;

public class OrderHistoryRepository
{
    private readonly string _connectionString;

    // Constructor que recibe la configuración
    public OrderHistoryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OrdersDatabase");
    }

    // Método para obtener las órdenes de la base de datos
    public List<OrdersHistory> GetOrders()
    {
        var orders = new List<OrdersHistory>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();  // Abre la conexión con la base de datos
            string query = "SELECT * FROM ORDERS_HISTORY";  // Consulta SQL

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())  // Ejecuta la consulta y obtiene los datos
                {
                    while (reader.Read())  // Lee cada fila del resultado
                    {
                        orders.Add(new OrdersHistory
                        {
                            TxNumber = Convert.ToInt32(reader["TX_NUMBER"]),  // Convierte el valor de TX_NUMBER a entero
                            OrderDate = Convert.ToDateTime(reader["ORDER_DATE"]),  // Convierte el valor de ORDER_DATE a DateTime
                            ActionOrder = reader["ACTION_ORDER"].ToString(),  // Convierte el valor de ACTION a cadena de texto
                            StatusOrder = reader["STATUS_ORDER"].ToString(),
                            Symbol = reader["SYMBOL"].ToString(),
                            Quantity = Convert.ToInt32(reader["QUANTITY"]),
                            Price = Convert.ToDecimal(reader["PRICE"]),
                        });
                    }
                }
            }
        }

        return orders;  // Retorna la lista de órdenes
    }
    
    // Método para obtener la órdenes completadas
    public List<OrdersHistory> GetCompletedOrders()
    {
        var orders = new List<OrdersHistory>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            // Consulta SQL para obtener las órdenes completadas (EXECUTED) y calcular el monto neto
            string query = @"SELECT TX_NUMBER, ORDER_DATE, ACTION_ORDER, STATUS_ORDER, SYMBOL, QUANTITY, PRICE, 
                         (QUANTITY * PRICE) AS TotalPrice 
                         FROM ORDERS_HISTORY 
                         WHERE STATUS_ORDER = 'EXECUTED'";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new OrdersHistory
                        {
                            TxNumber = Convert.ToInt32(reader["TX_NUMBER"]),
                            OrderDate = Convert.ToDateTime(reader["ORDER_DATE"]),
                            ActionOrder = reader["ACTION_ORDER"].ToString(),
                            StatusOrder = reader["STATUS_ORDER"].ToString(),
                            Symbol = reader["SYMBOL"].ToString(),
                            Quantity = Convert.ToInt32(reader["QUANTITY"]),
                            Price = Convert.ToDecimal(reader["PRICE"]),
                            // Aquí calculamos el TotalPrice sobre la marcha
                            TotalPrice = Convert.ToInt32(reader["QUANTITY"]) * Convert.ToDecimal(reader["PRICE"])
                        });
                    }
                }
            }
        }

        return orders;
    }

    // Método para obtener datos según el año que queramos.
    public List<OrdersHistory> GetOrdersByYear(int year)
    {
        var orders = new List<OrdersHistory>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            // Consulta SQL para obtener las órdenes de un año específico
            string query = @"SELECT TX_NUMBER, ORDER_DATE, ACTION_ORDER, STATUS_ORDER, SYMBOL, QUANTITY, PRICE, 
                         (QUANTITY * PRICE) AS TotalPrice 
                         FROM ORDERS_HISTORY 
                         WHERE YEAR(ORDER_DATE) = @Year";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Añadimos el parámetro @Year a la consulta
                command.Parameters.AddWithValue("@Year", year);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new OrdersHistory
                        {
                            TxNumber = Convert.ToInt32(reader["TX_NUMBER"]),
                            OrderDate = Convert.ToDateTime(reader["ORDER_DATE"]),
                            ActionOrder = reader["ACTION_ORDER"].ToString(),
                            StatusOrder = reader["STATUS_ORDER"].ToString(),
                            Symbol = reader["SYMBOL"].ToString(),
                            Quantity = Convert.ToInt32(reader["QUANTITY"]),
                            Price = Convert.ToDecimal(reader["PRICE"]),
                            TotalPrice = Convert.ToInt32(reader["QUANTITY"]) * Convert.ToDecimal(reader["PRICE"])
                        });
                    }
                }
            }
        }

        return orders;
    }

    // Método para agregar órdenes
    public void AddOrder(OrdersHistory newOrder)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            // Consulta SQL para insertar una nueva orden
            string query = @"INSERT INTO ORDERS_HISTORY (ORDER_DATE, ACTION_ORDER, STATUS_ORDER, SYMBOL, QUANTITY, PRICE) 
                         VALUES (@OrderDate, @ActionOrder, 'PENDING', @Symbol, @Quantity, @Price)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Añadimos los parámetros a la consulta
                command.Parameters.AddWithValue("@OrderDate", newOrder.OrderDate);
                command.Parameters.AddWithValue("@ActionOrder", newOrder.ActionOrder);
                command.Parameters.AddWithValue("@Symbol", newOrder.Symbol);
                command.Parameters.AddWithValue("@Quantity", newOrder.Quantity);
                command.Parameters.AddWithValue("@Price", newOrder.Price);

                command.ExecuteNonQuery();  // Ejecuta la consulta
            }
        }
    }

    // Método para modificar las órdenes cuyo status_order sea 'pending'.
    public bool UpdateOrderStatus(int txNumber, string newStatus)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            // Consulta SQL para actualizar el estado de la orden
            string query = @"UPDATE ORDERS_HISTORY 
                         SET STATUS_ORDER = @NewStatus 
                         WHERE TX_NUMBER = @TxNumber AND STATUS_ORDER = 'PENDING'";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Añadimos los parámetros a la consulta
                command.Parameters.AddWithValue("@NewStatus", newStatus);
                command.Parameters.AddWithValue("@TxNumber", txNumber);

                int affectedRows = command.ExecuteNonQuery(); // Ejecuta la consulta
                return affectedRows > 0; // Retorna true si se actualizó una fila
            }
        }
    }




}
