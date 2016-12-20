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
        }

        private static string[] SplitString(string str, char serparator)
        {
            switch (serparator)
            {
                case '.':
                    return str.Split('.').Select(s => SplitString(s, ',')).SelectMany(list => list, (strings, s) => s).ToArray();
                case ',':
                    return str.Replace("[", string.Empty).Replace("]", string.Empty).Split(',').Select(s => SplitString(s, '-')).SelectMany(list => list, (strings, s) => s).ToArray();
                case '-':
                    string[] rangeBounds = str.Split('-');
                    int start = int.Parse(rangeBounds[0]), end = int.Parse(rangeBounds[1]);
                    return Enumerable.Range(start, end - start).Select(n => n.ToString()).ToArray();
            }

            throw new Exception("Invalid endpoints range string");
        }
    }
}
