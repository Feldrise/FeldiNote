using FeldiNote.Api.Models;
using FeldiNote.Api.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeldiNote.Api.Helpers
{
    public class AuthenticationMiddleware
    {
        private readonly AuthenticationService _authenticationService;
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next, AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/notes"))
            {
                string authHeader = context.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic"))
                {
                    //Extract credentials
                    string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    string idAndPassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    int seperatorIndex = idAndPassword.IndexOf(':');

                    var id = idAndPassword.Substring(0, seperatorIndex);
                    var password = idAndPassword.Substring(seperatorIndex + 1);

                    if (_authenticationService.CheckCredential(id, password))
                    {
                        await _next.Invoke(context);
                    }
                    else
                    {
                        context.Response.StatusCode = 401; //Unauthorized
                        return;
                    }
                }
                else
                {
                    // no authorization header
                    context.Response.StatusCode = 401; //Unauthorized
                    return;
                }
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
