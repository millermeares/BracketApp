using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillerAPI.DataAccess
{
    internal static class ErrorDBString
    {
        internal static string LogException= 
            @"
        INSERT INTO error_log(errorKey, timeRecorded, message, callstack, category, exception_source)
        VALUES(@errorKey, now(6), @message, @callstack, @category, @exceptionSource);
            ";
    }
}
