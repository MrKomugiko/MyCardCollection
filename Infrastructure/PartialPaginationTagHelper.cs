using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace MyCardCollection.Infrastructure
{
    public class PartialPaginationTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "nav";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("aria-label", "Page navigation");
            output.Content.SetHtmlContent(AddPageContent());
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int PageRange { get; set; }
        public string PageFirst { get; set; }
        public string PageLast { get; set; }
        public string PageOnclickEvent { get; set; }


        private string AddPageContent()
        {
            InitDefaults();
            var content = new StringBuilder();
            InitPaginationHtml(content);

            if (PageNumber <= PageRange)
            {
                PageNumberUnderPageRange(content);
            }
            else if (PageNumber > PageRange && PageNumber < PageCount - PageRange)
            {
                PageNumberBetweenPageRange(content);
            }
            else
            {
                PageNumberUpperPageRange(content);
            }

            ClosingHtml(content);

            return content.ToString();
        }

        private void ClosingHtml(StringBuilder content)
        {
            if (PageNumber != PageCount)
            {
                content.Append($"<li class='page-item'><a class='page-link' onclick='{PageOnclickEvent}({PageCount})'>{PageLast}</a></li>");
            }
            content.Append(" </ul");
        }

        private void PageNumberUpperPageRange(StringBuilder content)
        {
            for (int currentPage = PageCount - (2 * PageRange); currentPage < PageCount + 1; currentPage++)
            {
                if (currentPage < 1 || currentPage > PageCount)
                {
                    continue;
                }
                var active = currentPage == PageNumber ? "active" : "";
                content.Append($"<li class='page-item {active}' style='z-index:0; style='z-index:0;'><a class='page-link'onclick='{PageOnclickEvent}({currentPage})'>{currentPage}</a></li>");
            }
        }

        private void PageNumberBetweenPageRange(StringBuilder content)
        {
            for (int currentPage = PageNumber - PageRange; currentPage < PageNumber + PageRange; currentPage++)
            {
                if (currentPage < 1 || currentPage > PageCount)
                {
                    continue;
                }
                var active = currentPage == PageNumber ? "active" : "";
                content.Append($"<li class='page-item {active}' style='z-index:0;'><a class='page-link'onclick='{PageOnclickEvent}({currentPage})'>{currentPage}</a></li>");
            }
        }

        private void PageNumberUnderPageRange(StringBuilder content)
        {
            for (int currentPage = 1; currentPage < 2 * PageRange + 1; currentPage++)
            {
                if (currentPage < 1 || currentPage > PageCount)
                {
                    continue;
                }
                var active = currentPage == PageNumber ? "active" : "";
                content.Append($"<li class='page-item {active}' style='z-index:0;'><a class='page-link'onclick='{PageOnclickEvent}({currentPage})'>{currentPage}</a></li>");
            }
        }

        private void InitPaginationHtml(StringBuilder content)
        {
            content.Append(" <ul class='pagination'>");
            if (PageNumber != 1)
            {
                content.Append($"<li class='page-item'><a class='page-link' onclick='{PageOnclickEvent}(1)'>{PageFirst}</a></li>");
            }
        }

        private void InitDefaults()
        {
            if (PageRange <= 0)
            {
                PageRange = 5;
            }

            if (PageCount < PageRange)
            {
                PageRange = PageCount;
            }

            if (PageSize <= 0)
            {
                PageSize = 10;
            }

            if (string.IsNullOrEmpty(PageFirst))
            {
                PageFirst = "First";
            }

            if (string.IsNullOrEmpty(PageLast))
            {
                PageLast = "Last";
            }
        }
    }
}