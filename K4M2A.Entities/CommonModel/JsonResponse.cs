using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritualNetwork.Entities.CommonModel
{
    public class JsonResponse
    {
        public bool Success { get; }

        public string Message { get; }

        public object Result { get; }

        public JsonResponse(int statusCode, bool? success = null, string message = null, object result = null)
        {
            Success = success ?? IsSuccessStatus(statusCode);
            Message = message ?? GetDefaultMessage(statusCode);
            Result = result;
        }

        private static bool IsSuccessStatus(int statusCode)
        {
            if (statusCode == 200)
                return true;
            else
                return false;
        }

        private static string GetDefaultMessage(int statusCode)
        {
            switch (statusCode)
            {
                case 401:
                    return "Unauthorized to perform this action";
                case 403:
                    return "Forbidden to perform this action";
                case 500:
                    return "An unhandled exception occurred";
                default:
                    return null;
            }
        }
    }
}
