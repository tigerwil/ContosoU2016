using ContosoU2016.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoU2016.Components
{
    public class NewsWidget:ViewComponent
    {
        private INewsService _newService;

        public NewsWidget(INewsService newsService)
        {
            _newService = newsService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int threshold = 3)
        {
            var news = await _newService.GetNews(threshold);
            return View(news);
        }



    }

    public class NewsItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
