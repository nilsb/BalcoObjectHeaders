﻿using System;
using System.Net;

namespace SPLib.Shared
{
    public class HttpResult : IDisposable
    {
        private HttpStatusCode _Status { get; set; }
        private string _Message { get; set; }

        public HttpResult(HttpStatusCode Status, string Message)
        {
            _Status = Status;
            _Message = Message;
        }

        public HttpStatusCode Status { get { return _Status; } }
        public string Message { get { return _Message; } }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
