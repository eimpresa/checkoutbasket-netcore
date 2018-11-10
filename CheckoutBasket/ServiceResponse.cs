using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket
{
    public class ServiceResponse
    {
        private ServiceResponse()
        {

        }

        public bool Success { get; private set; }
        public IEnumerable<string> Errors { get; private set; }

        public static ServiceResponse Fail(string[] errors)
        {
            return new ServiceResponse { Errors = errors };
        }

        public static ServiceResponse Ok()
        {
            return new ServiceResponse { Success = true };
        }

        public static ServiceResponse<T> Fail<T>(string[] errors)
        {
            return new ServiceResponse<T>(false, errors, default(T));
        }

        public static ServiceResponse<T> Ok<T>(T data)
        {
            return new ServiceResponse<T>(true, null, data);
        }
    }
}
