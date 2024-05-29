using ExporterEPUB.Helpers;

namespace ExporterEPUB.XHTMLBuilder.Content
{
    public class IntroXHTML : IContentXHTML
    {
        private readonly string _storyTitle;
        private readonly string _authorName;
        private readonly string _storyDescription;
        private readonly string _storyCategories;
        private readonly string _storyStatus;
        private readonly string _storyImagePath;

        public static readonly string LOGO_PATH = Path.Join("..", "Resources", "logo.png").InverseSlash();

        public IntroXHTML(string storyTitle, string authorName, string storyDescription, string storyCategories, string storyStatus, string storyImagePath)
        {
            _storyTitle = storyTitle;
            _authorName = authorName;
            _storyDescription = storyDescription;
            _storyCategories = storyCategories;
            _storyStatus = storyStatus;
            _storyImagePath = storyImagePath;
        }

        public LayoutXHTML SetContent(LayoutXHTML layout)
        {
            var company = new XHTMLElement("div");
            {
                var logo = new XHTMLElementInline("img", new Dictionary<string, string>()
                {
                    {"alt", "StoryLand" },
                    {"src",  LOGO_PATH }
                });
                var source = new XHTMLElement("h2", null, null, "www.storyland.gov");
                company.AddChild(logo);
                company.AddChild(source);
            }
            var story = new XHTMLElement("div");
            {
                var cover = new XHTMLElementInline("img", new Dictionary<string, string>() { { "alt", "Cover" }, { "src", _storyImagePath } });
                var title = new XHTMLElement("h1", text: _storyTitle);
                var author = new XHTMLElement("p", text: _authorName);
                var categories = new XHTMLElement("p", text: _storyCategories);
                var status = new XHTMLElement("p", text: _storyStatus);
                var description = new XHTMLElement("p", text: _storyDescription);
                story.AddChild(cover);
                story.AddChild(title);
                story.AddChild(author);
                story.AddChild(categories);
                story.AddChild(status);
                story.AddChild(description);
            }
            layout.Body.AddChild(company);
            layout.Body.AddChild(story);
            return layout;
        }
    }
}
