using System.Net.Sockets;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Scrubby.Web.Models;

namespace Scrubby.Web.Controllers;

public class ErrorController : Controller
{
    [HttpGet("exception")]
    public IActionResult Error()
    {
        var exceptionHandler = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var errorModel = new ErrorModel() { StatusCode = 500, TraceIdentifier = HttpContext.TraceIdentifier };
        
        if (exceptionHandler?.Error is SocketException or NpgsqlException)
            return View("DatabaseError", errorModel);
        
        return View("ExceptionError", errorModel);
    }

    [HttpGet("error")]
    public IActionResult GenericError(int statusCode) => View("GenericError", new ErrorModel() { StatusCode = statusCode, TraceIdentifier = HttpContext.TraceIdentifier });
}