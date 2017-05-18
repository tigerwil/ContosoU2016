using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoU2016.Components;

namespace ContosoU2016.Services
{
    public class NewsService : INewsService
    {
        static List<NewsItem> _news;

        static NewsService()
        {
            //Normally this class would connect to a Data Store 
            //(json, database, file, another service like API, etc)
            _news = new List<NewsItem>()
            {
                new NewsItem(){Id=1, Title="ContosoU launches free Academic Upgrading"},
                new NewsItem(){Id=2, Title="Business students creating political change"},
                new NewsItem(){Id=3, Title="ContosoU celebrating 50th anniversary"},
                new NewsItem(){Id=4, Title="Engineering student appears on Dragon's Den"},
                new NewsItem(){Id=5, Title="ContosoU launches new website"}
            };
        }

        public Task<IEnumerable<NewsItem>> GetNews(int threshold)
        {
            return Task.FromResult<IEnumerable<NewsItem>>(_news.OrderBy(x => Guid.NewGuid()).Take(threshold).ToList());
        }
    }
}
