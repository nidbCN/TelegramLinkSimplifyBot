using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace TelegramLinkSimplifyBot.Plugins.Builtin.Utils;
internal class QueryStringUtil
{
    public NameValueCollection? FilterQuery(string query, params string[] reserve)
    {
        if (reserve is null)
        {
            throw new ArgumentNullException(nameof(reserve));
        }

        if (reserve.Length == 0)
        {
            return new NameValueCollection();
        }

        var queryPair = HttpUtility.ParseQueryString(query);
        if (queryPair is null)
        {
            return new NameValueCollection();
        }

        if (reserve.Length == 1)
        {
            if (queryPair[reserve[0]] is { } value)
            {
                return new NameValueCollection(1) {
                    { reserve[0], value }
                };
            }
        }

        if (queryPair.Count / 2 > reserve.Length)
        {
            var result = new NameValueCollection(reserve.Length);
            foreach (var key in queryPair.AllKeys)
            {
                if (reserve.Any(x => x == key))
                {
                    result.Add(key, queryPair[key]);
                }
            }

            return result;
        }
        else
        {
            foreach (var key in queryPair.AllKeys)
            {
                if (!reserve.Any(x => x == key))
                {
                    queryPair.Remove(key);
                }
            }

            return queryPair;
        }
    }
}
