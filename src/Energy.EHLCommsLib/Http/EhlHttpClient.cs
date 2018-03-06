using System;
using System.Net;
using Energy.EHLCommsLib.Contracts.Responses;
using Energy.EHLCommsLib.Interfaces;
using Energy.EHLCommsLib.Interfaces.Http;
using Energy.EHLCommsLib.Models;
using Energy.EHLCommsLib.Models.Http;
using Newtonsoft.Json;

namespace Energy.EHLCommsLib.Http
{
    public class EhlHttpClient : IEhlHttpClient
    {
        private IHttpClient _httpClient;
        public EhlHttpClient(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public SwitchesApiResponse GetApiDataTemplate(string url, string rel)
        {
           return null;
        }
       
    }
}
