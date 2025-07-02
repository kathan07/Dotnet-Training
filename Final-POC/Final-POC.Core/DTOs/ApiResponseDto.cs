using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_POC.Core.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }         
        public string Message { get; set; }       
        public T Data { get; set; }

        public ApiResponseDto() { }

        public ApiResponseDto(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ApiResponseDto<T> SuccessResponse(T data, string message = "Success")
            => new ApiResponseDto<T>(true, message, data);

        public static ApiResponseDto<T> FailResponse(string message)
            => new ApiResponseDto<T>(false, message, default);
    }
}
