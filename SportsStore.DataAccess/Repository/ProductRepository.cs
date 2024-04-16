using SportsStore.DataAccess.Data;
using SportsStore.DataAccess.Repository.IRepository;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.DataAccess.Repository
{
    public class ProductRepository: Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(Product entity)
        {
            _db.Products.Update(entity);
        }
    }
}
