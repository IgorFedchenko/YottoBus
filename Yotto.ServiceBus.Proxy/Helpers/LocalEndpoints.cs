using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Yotto.ServiceBus.Proxy.Helpers
{
    public static class LocalEndpoints
    {
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
