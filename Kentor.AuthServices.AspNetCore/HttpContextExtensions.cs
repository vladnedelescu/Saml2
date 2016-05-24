﻿using Kentor.AuthServices.WebSso;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kentor.AuthServices.AspNetCore
{
    static class HttpContextExtensions
    {
        public async static Task<HttpRequestData> ToHttpRequestData(
            this HttpContext context,
            Func<byte[], byte[]> cookieDecryptor)
        {
            if(context == null)
            {
                return null;
            }

            IEnumerable<KeyValuePair<string, string[]>> formData = null;
            if(context.Request.Body != null && context.Request.Method.ToUpper() == "POST")
            {
                var formCollection = await context.Request.ReadFormAsync();
                formData = formCollection.Select(d => new KeyValuePair<string, string[]>(d.Key, d.Value.ToArray()));
            }

            var applicationRootPath = context.Request.PathBase.Value;
            if(string.IsNullOrEmpty(applicationRootPath))
            {
                applicationRootPath = "/";
            }

            return new HttpRequestData(
                context.Request.Method,
                new Uri(string.Concat(
                        context.Request.Scheme,
                        "://",
                        context.Request.Host.ToUriComponent(),
                        context.Request.PathBase.ToUriComponent(),
                        context.Request.Path.ToUriComponent(),
                        context.Request.QueryString.ToUriComponent())),
                applicationRootPath,
                formData,
                context.Request.Cookies,
                cookieDecryptor);
        }
    }
}