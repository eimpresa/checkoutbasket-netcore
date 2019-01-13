using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutBasket.Client
{
    public class ServiceResponse<T>
    {
        public ServiceResponse(bool ok, IEnumerable<string> errors)
        {
            Ok = ok;
            Errors = errors;
            Data = default(T);
        }

        public ServiceResponse(T data)
        {
            Ok = true;
            Data = data;
        }

        public bool Ok { get; }
        public IEnumerable<string> Errors { get; }
        public T Data { get; }
    }
}
