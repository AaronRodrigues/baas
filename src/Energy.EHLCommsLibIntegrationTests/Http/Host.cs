using System.Net;
using System.Net.Sockets;

namespace Energy.EHLCommsLibIntegrationTests.Http
{
    public static class  Host
    {
        private static string _currentIpv4Address;
        
        public static string Ipv4Address
        {
            get
            {
                if (_currentIpv4Address == null)
                {
                    SetIpv4Address();
                }
                return _currentIpv4Address;
            }
        }

        private static void SetIpv4Address()
        {
            try
            {
                foreach (var ipAddress in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                        _currentIpv4Address = ipAddress.ToString();
                }
            }
            catch
            {
                _currentIpv4Address = null;
            } 
        }
    }
}