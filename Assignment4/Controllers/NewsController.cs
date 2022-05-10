using Assignment4.Data;
using Assignment4.ElasticSearch;
using Assignment4.Models;
using Nest;
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
            int pageSize = 4;
            int pageIndex = page ?? 1;
            var articles = db.Articles.OrderByDescending(a => a.CreatedAt).ToList();
            var categories = db.Categories.ToList();
            IPagedList<Article> pagedArticles = articles.ToPagedList(pageIndex, pageSize);
            ViewBag.categories = categories;
            var newestArticle = new List<Article>();
            for (int i = articles.Count - 1; i > articles.Count - 5; i--)
            {
                newestArticle.Add(articles[i]);
            }
            ViewBag.newArticle = newestArticle;
            return View(pagedArticles);
        }

        [HttpGet]
        public ActionResult SearchResult(string keyword, int? page, string category_id)
        {
            if (page == null) page = 1;
            int pageSize = 4;
            int pageIndex = (page ?? 1);
            // search elastic
            var searchRequest = new SearchRequest<Article>();
            searchRequest.From = 0;
            searchRequest.Size = 10000;
            var listQuery = new List<QueryContainer>();
            if (!string.IsNullOrEmpty(keyword))
            {
                var query = new BoolQuery
                {
                    Should = new List<QueryContainer>
                        {
                             new MatchQuery{ Field = "title", Query = keyword},
                             new MatchQuery{ Field = "description", Query = keyword}
                        }
                };
                listQuery.Add(query);
            }
            if (!string.IsNullOrEmpty(category_id))
            {
                listQuery.Add(new MatchQuery { Field = "categoryId", Query = category_id });
            }
            searchRequest.Query = new QueryContainer(new BoolQuery
            {
                Must = listQuery
            });
            searchRequest.Sort = new List<ISort>
                {
                    new FieldSort { Field = "createdAt", Order = SortOrder.Descending }
                };
            var searchResult =
                Search.EsClient().Search<Article>(searchRequest);
            var articleList = searchResult.Documents.ToList();
            ViewBag.categoriesResult = db.Categories.ToList();
            ViewBag.categoryId = category_id;
            ViewBag.keyword = keyword;
            return View("SearchResult", articleList.ToPagedList(pageIndex, pageSize));
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