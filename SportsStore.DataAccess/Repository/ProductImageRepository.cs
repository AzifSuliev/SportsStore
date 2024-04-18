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
    public class ProductImageRepository: Repository<ProductImage>, IProductImageRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductImageRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public void Update(ProductImage entity)
        {
            _db.ProductImages.Update(entity);
        }
    }
}
