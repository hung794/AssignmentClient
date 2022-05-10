using Assignment4.Data;
using Assignment4.ElasticSearch;
using Assignment4.Models;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Assignment4.Controllers
{
    public class NewsController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();
        // GET: News
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 4;
            int pageIndex = (page ?? 1);
            Search.EsClient().Indices.Delete("articles");
            var articles = db.Articles.ToList();
            articles.Reverse();
            foreach(Article article in articles)
            { 
                Search.EsClient().IndexDocument(article);
            }
            var categories = db.Categories.ToList();
            IPagedList<Article> pagedArticles = articles.ToPagedList(pageIndex, pageSize);
            ViewBag.categories = categories;
            var newestArticle = new List<Article>();
            for(int i = articles.Count - 1; i > articles.Count - 5; i--)
            {
                newestArticle.Add(articles[i]);
            }
            ViewBag.newArticle = newestArticle;
            return View(pagedArticles);
        }

        [HttpGet]
        public ActionResult SearchResult(string keyword, int? page, string category_id)
        {
            var searchResponse = Search.EsClient().Search<Article>
                (
                        s => s.Query(q => q.Match(m => m.Field(f => f.CategoryId).Query(category_id)) ||
                        (q.Match(m => m.Field(f => f.Title).Query(keyword)) ||
                        q.Match(m => m.Field(f => f.Description).Query(keyword)) ||
                        q.Match(m => m.Field(f => f.Content).Query(keyword)))
                    )
                );
            ViewBag.categoriesResult = db.Categories.ToList();
            List<Article> articles1 = searchResponse.Documents.ToList();
            articles1.Reverse();
            if (page == null) page = 1;
            int pageSize = 4;
            int pageIndex = (page ?? 1);
            IPagedList<Article> pagedArticles = articles1.ToPagedList(pageIndex, pageSize);
            ViewBag.categoryId = category_id;
            ViewBag.keyword = keyword;
            return View("SearchResult", pagedArticles);
        }

        [Route("News/Article/{title}")]
        public ActionResult Article(string title)
        {
            ViewBag.categoryInArticle = db.Categories.ToList();
            var article = db.Articles.Find(title);
            var category = article.CategoryId;
            var listArticle = db.Articles.Where(q => q.CategoryId == category).ToList();
            var listArticleWithCategory = new List<Article>();
            if(listArticle.Count - 1 >= 4)
            {
                for(int i = 0; i < 4; i++)
                {
                    listArticleWithCategory.Add(listArticle[i]);
                }
            }
            else
            {
                listArticleWithCategory = listArticle;
            }
            var newsArticle = new List<Article>();
            for(int i = 0 ;i < 4; i++)
            {
                newsArticle.Add(db.Articles.ToList()[i]);
            }
            ViewBag.newArticle1 = newsArticle;
            ViewBag.ListArticle = listArticleWithCategory;
            return View(article);
        }
    }
}