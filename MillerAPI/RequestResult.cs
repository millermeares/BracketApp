using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillerAPI
{
    public class RequestResult
    {
        public bool Valid { get; set; }
        public object? Payload { get; set; } = null;
        private RequestResult(bool valid, object? payload)
        {
            Valid = valid;
            Payload = payload;
        }

        public static RequestResult New(bool valid, object? payload)
        {
            return new RequestResult(valid, payload);
        }
        public static RequestResult ErrorResult(object payload)
        {
            return new RequestResult(false, payload);
        }
        public static RequestResult OkResult(object? payload = null)
        {
            return new RequestResult(true, payload);
        }
    }
}
