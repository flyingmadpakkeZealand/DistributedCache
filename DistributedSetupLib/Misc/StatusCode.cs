using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedSetupLib.Misc
{
    public enum StatusCode
    {
        Ok = 200,
        Empty = 202,
        NotFound = 404
    }

    public static class StatusCodeMethod
    {
        public static int ToNumber(this StatusCode statusCode)
        {
            return (int) statusCode;
        }
    }
}
