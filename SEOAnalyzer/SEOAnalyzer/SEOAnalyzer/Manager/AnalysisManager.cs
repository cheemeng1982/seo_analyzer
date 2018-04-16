using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using SEOAnalyzer.Models;
using HtmlAgilityPack;
using System.Net.Http;

namespace SEOAnalyzer
{
    public class AnalysisManager
    {
        public bool IsUrlResponsePositive(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Timeout = 15000;
            request.Method = "HEAD"; 

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }

            }

            catch (WebException)
            {
                return false;
            }
        }

        public AnalysisResult AnalyzeContent(string s, string options, string url = "")
        {
            var optChecked = options.Split(',');
            var listStopWords = UtilityManager.ReadStopWordList();

            var inputString = RemoveStopWord(s, listStopWords);
            AnalysisResult ar = new AnalysisResult();

            foreach (var opt in optChecked)
            {
                int resOpt = -1;
                int.TryParse(opt, out resOpt);
                if (!string.IsNullOrEmpty(opt) && resOpt > -1)
                {
                    switch (resOpt)
                    {
                        case (int)AnalysisOption.NumberOccurance:
                            ar.FrequencyOccurance = GetNumberOccurance(inputString);
                            break;
                        case (int)AnalysisOption.MetaTagSearch:
                            ar.MetaTagSearchResult = GetMetaTagSearch(inputString);
                            break;
                        case (int)AnalysisOption.ExternalLink:
                            ar.ExternalLinks = GetExternalLinks(s, url);
                            break;
                    }
                }
            }
            return ar;
        }

        public AnalysisResult WebScraping(string path, string options)
        {
            if (IsUrlResponsePositive(path))
            {
                WebClient w = new WebClient();
                string s = w.DownloadString(path);

                var res = AnalyzeContent(s, options, path);

                return res;
            }
            else
            {
                throw new Exception(string.Format("The given url is not a positive response Url - \"{0}\"", path));
            }
        }

        private string RemoveStopWord(string inputString, List<string> listStopWords)
        {
            // Convert our input to lowercase
            inputString = inputString.ToLower();

            // Split on spaces into a List of strings
            List<string> wordList = inputString.Split(' ').ToList();

            foreach (string word in listStopWords)
            {
                while (wordList.Contains(word))
                {
                    wordList.Remove(word);
                }
            }
            
            return String.Join(" ", wordList.ToArray());
        }

        private List<WordResult> GetNumberOccurance(string inputString)
        {
            // Convert our input to lowercase
            inputString = inputString.ToLower();

            // Characters to strip from the input and do it
            string[] stripChars = { ";", ",", ".", "-", "_", "^", "(", ")", "[", "]",
                        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "\n", "\t", "\r" };
            foreach (string character in stripChars)
            {
                inputString = inputString.Replace(character, "");
            }

            // Split on spaces into a List of strings
            List<string> wordList = inputString.Split(' ').ToList();

            List<WordResult> newRes = new List<WordResult>();

            foreach (string word in wordList)
            {
                if (!string.IsNullOrEmpty(word.Trim()))
                {
                    // If the length of the word is at least three letters...
                    if (word.Length >= 3)
                    {
                        if (newRes.Any(w => w.Word == word.Trim()))
                        {
                            newRes.First(w => w.Word == word.Trim()).Frequency++;
                        }
                        else
                        {
                            newRes.Add(new WordResult() { Word = word.Trim(), Frequency = 1 });
                        }
                    }
                }
            }

            if(newRes.Count() > 0)
            {
                newRes = newRes.OrderByDescending(w => w.Frequency).ToList();
            }

            return newRes;
        }

        private List<WordResult> GetMetaTagSearch(string inputString)
        {
            //var webGet = new HtmlWeb();
            //var document = webGet.Load(url);

            var document = new HtmlDocument();
            document.LoadHtml(inputString);

            List<WordResult> newRes = new List<WordResult>();
       
            var metaTags = document.DocumentNode.SelectNodes("//meta");
            if (metaTags != null)
            {
                string meta = string.Empty;
                foreach (var tag in metaTags)
                {
                    if (tag.Attributes["name"] != null && tag.Attributes["content"] != null)
                    {
                        string val = tag.Attributes["content"].Value.ToLower();

                        if (!string.IsNullOrEmpty(val))
                        {
                            meta = val + " " + meta;
                        }
                    }
                }

                newRes = GetNumberOccurance(meta);
            }

            return newRes;
        }

        private List<LinkItem> GetExternalLinks(string inputString, string urlGiven)
        {
            string localDomain = string.IsNullOrEmpty(urlGiven)? "" : GetDomainName(urlGiven);
            var extLinks = LinkFinder.Find(inputString);
            
            if(extLinks != null)
            extLinks = extLinks.Where(i => !string.IsNullOrEmpty(i.Href) && (string.IsNullOrEmpty(localDomain) ? true : !i.Href.Contains(localDomain)) && (i.Href.Contains("http") || i.Href.Contains("https"))).ToList();

            return extLinks.ToList();
        }

        private string GetDomainName(string Url)
        {
            if (!Url.Contains("://"))
                Url = "http://" + Url;

            return new Uri(Url).Host;
        }
 
    }
}