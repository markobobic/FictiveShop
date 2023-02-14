using Ardalis.GuardClauses;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FictiveShop.Infrastructure.Repositories
{
    public class ProductsRepository : IRepository<Product>
    {
        private readonly FictiveShopDbContext _db;

        public ProductsRepository(FictiveShopDbContext db)
        {
            _db = db;
        }

        public void Create(Product product)
        {
            if (product is null) Guard.Against.Null(product, nameof(product));
            _db.Products.Add(product);
        }

        public IReadOnlyCollection<Product> GetAll() => _db.Products;

        public IReadOnlyCollection<Product> GetAll(Func<Product, bool> filter) =>
            _db.Products.Where(filter).ToList();

        public Product GetById(string id) => _db.Products.FirstOrDefault(x => x.Id == id);

        public void Remove(string id)
        {
            var product = _db.Products.FirstOrDefault(x => x.Id == id);
            if (product is null) Guard.Against.Null(product, nameof(product));

            _db.Products.Remove(product);
        }

        public void Update(Product newProduct)
        {
            var oldProduct = _db.Products.FirstOrDefault(x => x.Id == newProduct.Id);
            if (oldProduct is null) Guard.Against.Null(oldProduct, nameof(oldProduct));

            _db.Products.ReplaceOne(newProduct, oldProduct);
        }
    }
}