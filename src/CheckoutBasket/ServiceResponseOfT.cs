using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket
{
    public class ServiceResponse<T>
    {
        public ServiceResponse(bool success, IEnumerable<string> errors, T data)
        {
            Success = success;
            Errors = errors;
            Data = data;
        }

        public bool Success { get; private set; }
        public IEnumerable<string> Errors { get; private set; }
        public T Data { get; private set; }
    }
}
