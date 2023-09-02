////using System;
////using System.Collections.Generic;
////using System.Net;
////using System.Net.Http;
////using System.Threading;
////using System.Threading.Tasks;

////namespace AGBrand.Packages.Handlers
////{
////    public class HttpClientFactoryInterceptor : DelegatingHandler
////    {
////        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;

////        public HttpClientFactoryInterceptor()
////        {
////            _handlerFunc = (request, cancellationToken) => Task.FromResult(request.CreateResponse(HttpStatusCode.OK));
////        }

////        public HttpClientFactoryInterceptor(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
////        {
////            _handlerFunc = handlerFunc;
////        }

////        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
////        {
////            return _handlerFunc(request, cancellationToken);
////        }
////    }
////}
