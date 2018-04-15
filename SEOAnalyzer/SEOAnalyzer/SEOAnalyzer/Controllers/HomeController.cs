using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SEOAnalyzer.Models;
using System.Net.Http;

namespace SEOAnalyzer.Controllers
{
    public class HomeController : Controller
    {
        List<string> GetStopWords()
        {
            var noms = System.Runtime.Caching.MemoryCache.Default["listStopWord"];
            if (noms == null)
            {
                noms = UtilityManager.ReadStopWordList();
                System.Runtime.Caching.MemoryCache.Default["listStopWord"] = noms;
            }

            return ((List<string>)noms);
        }

        public ActionResult Index()
        {
            var list = UtilityManager.ReadStopWordList();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public JsonResult GetAnalyzeResult(string options, int mode, string content)
        {
            try
            {
                AnalysisManager am = new AnalysisManager();

                AnalysisResult res = null;
                content = HttpUtility.UrlDecode(content);
                if (mode == 1)
                {
                    res = am.AnalyzeContent(content, options);
                }
                else
                {
                    res = am.WebScraping(content, options);
                }

                return Json(new
                {
                    status = "success",
                    aaData = res
                },
                    JsonRequestBehavior.DenyGet);
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    status = "error",
                    message = ex.Message
                },
                   JsonRequestBehavior.DenyGet);
            }
        }
    }
}