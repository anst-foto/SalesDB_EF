using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SalesDB_EF.Models;
using AppContext = SalesDB_EF.Models.AppContext;

namespace SalesDB_EF.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly AppContext _db;
    
    public ObservableCollection<Sale> Sales { get; } = [];
    public ObservableCollection<Product> Products { get; } = [];

    [Reactive] public string? ProductName { get; set; }
    [Reactive] public decimal? ProductPrice { get; set; }
    
    [Reactive] public int? SaleAmount { get; set; }
    [Reactive] public DateTimeOffset? SaleDate { get; set; }
    [Reactive] public Product? SelectedProduct { get; set; }

    [Reactive] public bool IsFormAddProduct { get; set; }
    [Reactive] public bool IsFormAddSale { get; set; }

    public ReactiveCommand<Unit, Unit> CommandRefresh { get; }
    public ReactiveCommand<Unit, Unit> CommandAddProduct { get; }
    public ReactiveCommand<Unit, Unit> CommandAddSale { get; }
    public ReactiveCommand<string, Unit> CommandSave { get; }

    public MainWindowViewModel()
    {
        var connectionString = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build()
#if DEBUG
            .GetConnectionString("ConnectionToTestDB");
#elif RELEASE
            .GetConnectionString("ConnectionToPublicDB");
#endif
        _db = new AppContext(connectionString);

        Refresh();
        IsFormAddProduct = false;
        IsFormAddSale = false;

        CommandRefresh = ReactiveCommand.Create(Refresh);

        CommandAddSale = ReactiveCommand.Create(() =>
        {
            IsFormAddSale = true;
            IsFormAddProduct = false;
        });

        CommandAddProduct = ReactiveCommand.Create(() =>
        {
            IsFormAddSale = false;
            IsFormAddProduct = true;
        });

        CommandSave = ReactiveCommand.Create<string, Unit>(Save);
    }

    private void Refresh()
    {
        Sales.Clear();
        Products.Clear();

        _db
            .Sales
            .ToList()
            .ForEach(s => Sales.Add(s));
        
        _db
            .Products
            .ToList()
            .ForEach(p => Products.Add(p));
    }

    private Unit Save(string name)
    {
        switch (name)
        {
            case "AddProduct":
                _db.Products.Add(new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = ProductName,
                    Price = ProductPrice.Value
                });
                break;

            case "AddSale":
                _db.Sales.Add(new Sale()
                {
                    Id = Guid.NewGuid(),
                    Product = SelectedProduct!,
                    Amount = SaleAmount!.Value,
                    Date = SaleDate!.Value.DateTime.ToUniversalTime()
                });
                break;
        }

        _db.SaveChanges();

        return new Unit();
    }
}