namespace RandomQuotes.Web.Models
{
    public class Quote
    {
        public static List<string> Quotes = new List<string>();
        public static List<string> Authors = new List<string>();

        // In-memory only, resets on pod restart/redeploy - fine for a demo counter,
        // not meant to be a durable metric. Interlocked keeps it safe under concurrent requests.
        private static long _viewCount = 0;

        public static void Initialize()
        {
            var quoteFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{Path.DirectorySeparatorChar}data{Path.DirectorySeparatorChar}quotes.txt");
            Quotes = File.Exists(quoteFilePath) ? File.ReadAllLines(quoteFilePath).Select(System.Net.WebUtility.HtmlDecode).ToList() : new List<string>();
            var authorFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{Path.DirectorySeparatorChar}data{Path.DirectorySeparatorChar}authors.txt");
            Authors = File.Exists(authorFilePath) ? File.ReadAllLines(authorFilePath).Select(System.Net.WebUtility.HtmlDecode).ToList() : new List<string>();
        }

        private Quote(string text, string author, long viewCount)
        {
            QuoteText = text;
            Author = author;
            ViewCount = viewCount;
        }

        public static Quote GetRandomQuote()
        {
            var viewCount = Interlocked.Increment(ref _viewCount);

            var random = new Random();
            var index = random.Next(Quotes.Count);

            var randomQuote = Quotes.ElementAtOrDefault(index);
            var randomAuthor = Authors.ElementAtOrDefault(index);

            if (string.IsNullOrEmpty(randomQuote) | string.IsNullOrEmpty(randomAuthor))
            {
                return BuildQuote("Something went wrong", "System", viewCount);
            }

            return BuildQuote(randomQuote, randomAuthor, viewCount);
        }

        public static Quote BuildQuote(string quote, string author, long viewCount)
        {
            return new Quote(quote, author, viewCount);
        }

        public string QuoteText { get; set; }
        public string Author { get; set; }
        public long ViewCount { get; set; }
    }
}
