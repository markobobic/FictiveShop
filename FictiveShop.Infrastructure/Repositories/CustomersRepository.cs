using Ardalis.GuardClauses;
using Castle.Core.Resource;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FictiveShop.Infrastructure.Repositories
{
    public class CustomersRepository : IRepository<Customer>
    {
        private readonly FictiveShopDbContext _db;

        public CustomersRepository(FictiveShopDbContext db)
        {
            _db = db;
        }

        public void Create(Customer entity)
        {
            if (entity is null)
                Guard.Against.Null(entity, nameof(entity), "The customer object was null when attempting to add it.");
            _db.Customers.Add(entity);
        }

        public IReadOnlyCollection<Customer> GetAll() => _db.Customers.ToList();

        public IReadOnlyCollection<Customer> GetAll(Func<Customer, bool> filter) => _db.Customers.Where(filter).ToList();

        public Customer GetById(string id) => _db.Customers.FirstOrDefault(x => x.Id == id);

        public void Remove(string id)
        {
            var customer = _db.Customers.FirstOrDefault(x => x.Id == id);
            if (customer is null) Guard.Against.Null(customer, nameof(customer), "The customer object was null when attempting to remove it.");

            _db.Customers.Remove(customer);
        }

        public void Update(Customer newCustomer)
        {
            var oldCustomer = _db.Customers.FirstOrDefault(x => x.Id == newCustomer.Id);
            if (oldCustomer is null) Guard.Against.Null(oldCustomer, nameof(oldCustomer));

            _db.Customers.ReplaceOne(newCustomer, oldCustomer);
        }
    }
}