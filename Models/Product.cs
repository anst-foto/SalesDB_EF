using System;
using System.Collections.Generic;

namespace SalesDB_EF.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    public List<Sale> Sales { get; set; }
}