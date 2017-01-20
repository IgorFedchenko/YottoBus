using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Yotto.ServiceBus.Proxy.Helpers
{
    /// <summary>
    /// Contains methods for searching local endpoints
    /// </summary>
    public static class LocalEndpoints
    {
        /// <summary>
        /// Gets the TCP endpoints list on localhost.
        /// </summary>
        /// <returns></returns>
        public static  List<IPEndPoint> GetTcpEndpoints()
        {
            var tcpEndpoints = new List<IPEndPoint>();

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            IEnumerator myEnum = tcpConnInfoArray.GetEnumerator();

            while (myEnum.MoveNext())
            {
                TcpConnectionInformation tcpInfo = (TcpConnectionInformation)myEnum.Current;
                tcpEndpoints.Add(new IPEndPoint(IPAddress.Loopback, tcpInfo.LocalEndPoint.Port));
            }

            return tcpEndpoints;
        }
    }
}
