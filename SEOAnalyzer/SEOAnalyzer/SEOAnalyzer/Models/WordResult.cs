using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEOAnalyzer.Models
{
    public class WordResult
    {
        public string Word;
        public int Frequency;
    }

    public class AnalysisResult
    {
        public List<WordResult> FrequencyOccurance = new List<WordResult>();
        public List<LinkItem> ExternalLinks = new List<LinkItem>();
        public List<WordResult> MetaTagSearchResult = new List<WordResult>();
    }
}