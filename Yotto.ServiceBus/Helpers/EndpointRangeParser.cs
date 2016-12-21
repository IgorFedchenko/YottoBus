using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Yotto.ServiceBus.Helpers
{
    public static class EndpointsRangeParser
    {
        public static List<IPEndPoint> Parse(string endpointsRange)
        {
            string[] ipAndPort = endpointsRange.Split(':');

            string[] ports = ParseList(ipAndPort[1]).SelectMany(ParseRange, (range, port) => port).Distinct().ToArray();

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
            string[] ips = CartesianProduct(newOctetLists).Select(octets => string.Join(".", octets)).Distinct().ToArray();

            List<IPEndPoint> endpoints = CartesianProduct(new [] { ips, ports }).Select(parts => BuildEndpoint(parts.First(), parts.Last())).ToList();
            return endpoints;
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
