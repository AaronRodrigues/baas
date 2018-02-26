﻿using System.Collections.Generic;
using System.Net;
using System.Text;
using CTM.Energy.Common.Interfaces;
using Energy.EHLCommsLib.Interfaces;
using Microsoft.Practices.ObjectBuilder2;

namespace Energy.EHLCommsLibIntegrationTests.Http
{
    public class HttpClient : IHttpClient
    {
        private bool _allowRedirect = true;
        private Dictionary<string, string> _headerFields = new Dictionary<string, string>();
        
        public HttpClient()
        {
            TimeOutValue = (45*1000);

            //if(!string.IsNullOrWhiteSpace(AppSettings.WebClientProxy))
            //{
            //    WebProxy = new WebProxy(AppSettings.WebClientProxy);
            //}

        }

        public int? TimeOutValue;
        public readonly WebProxy WebProxy;

        public string ContentType { get; set; }
        public string AcceptHeader { get; set; }
        public string AuthorizationToken { get; set; }

        public void AddHeader(string key, string value)
        {
            _headerFields.Add(key, value);
        }

        public IResponse Get(string requestUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(requestUrl);

            ConfigureRequest(request);

            return new HttpResponse((HttpWebResponse)request.GetResponse());
        }

        public IResponse Post(string requestUrl, string requestData)
        {
            var request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "POST";

            ConfigureRequest(request);

            using (var stream = request.GetRequestStream())
            {
                var requestBody = Encoding.ASCII.GetBytes(requestData);
                stream.Write(requestBody, 0, requestBody.Length);
            }

            return new HttpResponse((HttpWebResponse)request.GetResponse());
        }

        public IHttpClient WithTimeOutOf(int milliseconds)
        {
            TimeOutValue = milliseconds;
            return this;
        }

        public IHttpClient WithoutRedirect()
        {
            _allowRedirect = false;
            return this;
        }

        private void ConfigureRequest(HttpWebRequest request)
        {
            if (TimeOutValue.HasValue)
            {
                request.Timeout = TimeOutValue.Value;
            }

            if (WebProxy != null)
            {
                request.Proxy = WebProxy;
            }

            if (ContentType != null)
            {
                request.ContentType = ContentType;
            }

            if (AcceptHeader != null)
            {
                request.Accept = AcceptHeader;
            }

            if (_headerFields.Count > 0)
            {
                _headerFields.ForEach(x=> request.Headers.Add(x.Key, x.Value));
            }

            if (!string.IsNullOrWhiteSpace(AuthorizationToken))
                request.Headers.Add("Authorization", AuthorizationToken);

            request.AllowAutoRedirect = _allowRedirect;
        }
    }
}
