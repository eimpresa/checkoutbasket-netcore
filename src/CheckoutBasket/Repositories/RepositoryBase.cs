using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CheckoutBasket.Repositories
{
    abstract public class RepositoryBase<T, TKey> : IRepositoryBase<T, TKey>
    {
        private readonly Func<T, TKey> keyAccessor;

        protected RepositoryBase(Func<T, TKey> keyAccessor)
        {
            this.keyAccessor = keyAccessor;
            this.Data = new Dictionary<TKey, T>();
        }

        protected IDictionary<TKey, T> Data { get; private set; }

        public Task Add(T model)
        {
            var key = keyAccessor.Invoke(model);
            Data.Add(key, model);
            return Task.CompletedTask;
        }

        public Task Clear()
        {
            Data.Clear();
            return Task.CompletedTask;
        }

        public Task Delete(T model)
        {
            var key = keyAccessor.Invoke(model);
            Data.Remove(key);
            return Task.CompletedTask;
        }

        public Task<T> GetById(TKey id)
        {
            if (Data.TryGetValue(id, out T result))
            {
                return Task.FromResult(result);
            }

            return Task.FromResult(default(T));
        }
    }
}