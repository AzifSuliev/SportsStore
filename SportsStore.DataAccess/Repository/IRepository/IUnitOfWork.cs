using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICategoryRepository Category { get; }
        public IProductRepository Product { get; }
        public IProductImageRepository ProductImage { get; }
        public IShoppingCartRepository ShoppingCart { get;}
        public IApplicationUserRepository ApplicationUser { get;}
        public IOrderHeaderRepository OrderHeader { get;}
        public IOrderDetailRepository OrderDetail { get;}
        void Save();
    }
}
