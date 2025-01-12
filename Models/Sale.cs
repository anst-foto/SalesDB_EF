using System;

namespace SalesDB_EF.Models;

public class Sale
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    public decimal Amount { get; set; }
}