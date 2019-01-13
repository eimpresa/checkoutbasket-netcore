using System;
using System.Collections.Generic;
using System.Text;

namespace CheckoutBasket.Client
{
    public class ServiceResponse
    {
        public ServiceResponse(bool ok, IEnumerable<string> errors)
        {
            Ok = ok;
            Errors = errors;
        }

        public bool Ok { get; }
        public IEnumerable<string> Errors { get; }
    }
}
