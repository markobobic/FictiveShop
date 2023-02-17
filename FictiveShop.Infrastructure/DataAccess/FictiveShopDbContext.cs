using FictiveShop.Core.Domain;
using FictiveShop.Core.ValueObjects;
using System;
using System.Collections.Generic;

namespace FictiveShop.Infrastructure.DataAccess
{
    public class FictiveShopDbContext
    {
        public List<Customer> Customers { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        public List<Product> Products { get; set; } = new();

        private Dictionary<Type, object> _tables = new Dictionary<Type, object>();

        public FictiveShopDbContext()
        {
            LoadCustomers();
            LoadProducts();
            _tables.Add(typeof(Customer), Customers);
            _tables.Add(typeof(Order), Orders);
            _tables.Add(typeof(Product), Products);
        }

        public List<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            if (_tables.TryGetValue(typeof(TEntity), out object list))
            {
                return (List<TEntity>)list;
            }
            else
            {
                throw new ArgumentException($"Entity type {typeof(TEntity).Name} is not supported.");
            }
        }

        public void SaveChanges() => true;

        private void LoadProducts()
        {
            var products = new List<Product>()
            {
                new Product
                {
                    Id = "fe9d5ba6-ac66-11ed-afa1-0242ac120002",
                    Name = "Hleb",
                    Price = new Price()
                    {
                        Amount = 60,
                        Currency = "RSD"
                    },
                    Quantity = 40
                },
                 new Product
                {
                    Name = "Vinjak",
                    Price = new Price()
                    {
                        Amount = 1060,
                        Currency = "RSD"
                    },
                    Quantity = 50
                },
            };
            Products.AddRange(products);
        }

        private void LoadCustomers()
        {
            var customers = new List<Customer>()
            {
                new Customer()
                {
                Id = "c2b09f04-ac66-11ed-afa1-0242ac120002",
                Address = new()
                {
                    City = "Belgrade",
                    HouseNumber = "12",
                    Street = "Street 123"
                },
                Name = new() { FirstName = "Milan", LastName = "Milicic" },
                PhoneNumber = "213-424-41"
                },
                 new Customer()
                {
                Address = new()
                {
                    City = "Belgrade",
                    HouseNumber = "14",
                    Street = "Street 444"
                },
                Name = new() { FirstName = "Goran", LastName = "Goranovic" },
                PhoneNumber = "213-424-42"
                }
            };
            Customers.AddRange(customers);
        }
    }
}