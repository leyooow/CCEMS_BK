using Application.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Helpers
{
    public static class ResponseHelper
    {
        public static GenericResponse<T> SuccessResponse<T>(T data, string message = "Operation successful")
        {
            return new GenericResponse<T>(true, message, 200, data);
        }
        public static GenericResponse<T> SuccessResponse<T>(string message = "Operation successful")
        {
            return new GenericResponse<T>(true, message, 200, default);
        }

        public static GenericResponse<T> FailResponse<T>(T data, string message = "Operation is not successful")
        {
            return new GenericResponse<T>(false, message, 200, data);
        }

        public static GenericResponse<T> ErrorResponse<T>(string message, int statusCode = 400)
        {
            return new GenericResponse<T>(false, message, statusCode);
        }
    }

}
