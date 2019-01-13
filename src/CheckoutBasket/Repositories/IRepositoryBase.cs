using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Repositories
{
    public interface IRepositoryBase<T, TKey>
    {
        Task Add(T model);
        Task Clear();
        Task Delete(T model);
        Task<T> GetById(TKey id);
    }
}
