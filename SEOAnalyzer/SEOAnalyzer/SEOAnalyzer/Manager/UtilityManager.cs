using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;

namespace SEOAnalyzer
{
    public static class UtilityManager
    {
        private static string PATH_STOP_WORD_LIST = ConfigurationManager.AppSettings["StopWordListPath"]; //  "~/listStopWord.txt";
        public static List<string> ReadStopWordList()
        {
            List<string> listStopWord = new List<string>();
            using (var filestream = new System.IO.FileStream(HttpContext.Current.Server.MapPath(PATH_STOP_WORD_LIST),
                                          System.IO.FileMode.Open,
                                          System.IO.FileAccess.Read))
            {

                var file = new System.IO.StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);
                string lineOfText = string.Empty;
                while ((lineOfText = file.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(lineOfText))
                        listStopWord.Add(lineOfText.Trim());
                }
            }
            return listStopWord;
        }
    }
}