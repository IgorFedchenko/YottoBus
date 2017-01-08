using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Yotto.ServiceBus.Proxy.Helpers
{
    public static class EndpointsRangeParser
    {
        public static List<IPEndPoint> Parse(string endpointsRange)
        {
            endpointsRange = endpointsRange.Replace("localhost", "127.0.0.1");

            string[] ipAndPort = endpointsRange.Split(':');

            string[] ports = ParseList(ipAndPort[1]).SelectMany(ParseRange, (range, port) => port).Distinct().ToArray();
            ValidatePorts(ports);

            string[] octetsRegions = ipAndPort[0].Split('.');
            List<string[]> octetLists = octetsRegions.Select(ParseList).ToList();
            List<List<string>> newOctetLists = new List<List<string>>();
            foreach (var octetList in octetLists.ToList())
            {
                List<string> newOctetsList = new List<string>();
                foreach (var octetRange in octetList)
                {
                    newOctetsList = newOctetsList.Concat(ParseRange(octetRange)).ToList();
                }
                newOctetLists.Add(newOctetsList);
            }

            ValidateIps(newOctetLists);

            string[] ips = CartesianProduct(newOctetLists).Select(octets => string.Join(".", octets)).Distinct().ToArray();

            List<IPEndPoint> endpoints = CartesianProduct(new [] { ips, ports }).Select(parts => BuildEndpoint(parts.First(), parts.Last())).ToList();
            return endpoints;
        }

        private static void ValidatePorts(string[] ports)
        {
            const string errorText = "Invalid port value: {0}: each port should be a number greater then 0";
            foreach (var port in ports)
            {
                int value;
                if (!int.TryParse(port, out value) || value <= 0)
                {
                    throw new ArgumentException(string.Format(errorText, port));
                }
            }
        }

        private static void ValidateIps(List<List<string>> newOctetLists)
        {
            const string errorText = "Invalid IP octet value: {0}: each octet should be a number in 0-254 range";
            IEnumerable<string> allOctets = newOctetLists.SelectMany(list => list, (list, elem) => elem);
            foreach (var octet in allOctets)
            {
                int value;
                if (!int.TryParse(octet, out value) || value < 0 || value > 254)
                {
                    throw new ArgumentException(string.Format(errorText, octet));
                }
            }
        }

        private static IPEndPoint BuildEndpoint(string ip, string port)
        {
            return new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
        }

        private static string[] ParseRange(string rangeStr)
        {
            string[] rangeBounds = rangeStr.Split('-');
            if (rangeBounds.Length > 1)
            {
                int start = int.Parse(rangeBounds[0]), end = int.Parse(rangeBounds[1]);
                return Enumerable.Range(start, end - start + 1).Select(n => n.ToString()).ToArray();
            }
            else
            {
                return new string[] { rangeBounds[0] };
            }
        }

        private static string[] ParseList(string listStr)
        {
            return listStr.Replace("[", string.Empty).Replace("]", string.Empty).Split(',');
        }

        private static IEnumerable<IEnumerable<T>> CartesianProduct<T>(IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                    from accseq in accumulator
                    from item in sequence
                    select accseq.Concat(new[] { item })
                );
        }
    }
}
