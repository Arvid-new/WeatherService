using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherService.Error
{
    public class ErrorInfo
    {
        public int StatusCode;
        public string Message;

        public ErrorInfo(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public ObjectResult ToObjectResult(string message)
        {
            return new ObjectResult(this)
            {
                StatusCode = StatusCode
            };
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static ObjectResult BadRequest(string message)
        {
            return Status(StatusCodes.Status400BadRequest, message);
        }

        public static ObjectResult ServiceUnvailable(string message)
        {
            return Status(StatusCodes.Status503ServiceUnavailable, message);
        }

        public static ObjectResult Status(int code, string message)
        {
            return new ObjectResult(new ErrorInfo(code, message))
            {
                StatusCode = code
            };
        }
    }
}
