using ContosoU2016.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU2016.Services
{
    public interface INewsService
    {
        Task<IEnumerable<NewsItem>> GetNews(int threshold);

        //IEnumerable<NewsItem> GetNews(int threshold);
    }
}
