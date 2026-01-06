using System.Collections.Concurrent;

public class Solution
{
    private List<string> _urlsToProcess = new List<string>();
    private readonly ConcurrentDictionary<string, byte> _seenUrls = new ConcurrentDictionary<string, byte>();
    private string host;

    public IList<string> Crawl(string startUrl, HtmlParser htmlParser)
    {
        var host = startUrl.Split('/')[2];

        _urlsToProcess.Add(startUrl);

        while (_urlsToProcess.Count > 0)
        {
            ConcurrentQueue<string> _newBatchToProcess = new ConcurrentQueue<string>();
            Parallel.ForEach(_urlsToProcess, (nextUrl) =>
            {
                if (_seenUrls.ContainsKey(nextUrl) || nextUrl.Split('/')[2] != host) return;
                if (!_seenUrls.TryAdd(nextUrl, 0)) return;
                var newUrls = htmlParser.GetUrls(nextUrl);
                foreach (var item in newUrls)
                {
                    _newBatchToProcess.Enqueue(item);
                }
            });
            _urlsToProcess.Clear();
            while (_newBatchToProcess.TryDequeue(out var nextUrl))
            {
                _urlsToProcess.Add(nextUrl);
            }
        }

        return _seenUrls.Keys.ToList();
    }
}
