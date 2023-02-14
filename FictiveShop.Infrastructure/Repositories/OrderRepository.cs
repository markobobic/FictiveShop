using Ardalis.GuardClauses;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FictiveShop.Infrastructure.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly FictiveShopDbContext _db;

        public OrderRepository(FictiveShopDbContext db)
        {
            _db = db;
        }

        public void Create(Order entity)
        {
            if (entity is null)
                Guard.Against.Null(entity, nameof(entity), "The order object was null when attempting to add it.");
            _db.Orders.Add(entity);
        }

        public IReadOnlyCollection<Order> GetAll() => _db.Orders.ToList();

        public IReadOnlyCollection<Order> GetAll(Func<Order, bool> filter) => _db.Orders.Where(filter).ToList();

        public Order GetById(string id) => _db.Orders.FirstOrDefault(x => x.Id == id);

        public void Remove(string id)
        {
            var order = _db.Orders.FirstOrDefault(x => x.Id == id);
            if (order is null) Guard.Against.Null(order, nameof(order));

            _db.Orders.Remove(order);
        }

        public void Update(Order newOrder)
        {
            var oldOrder = _db.Orders.FirstOrDefault(x => x.Id == newOrder.Id);
            if (oldOrder is null) Guard.Against.Null(oldOrder, nameof(oldOrder));

            _db.Orders.ReplaceOne(newOrder, oldOrder);
        }
    }
}