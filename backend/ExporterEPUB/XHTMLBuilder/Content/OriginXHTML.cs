using ExporterEPUB.Helpers;

namespace ExporterEPUB.XHTMLBuilder.Content
{
    public class OriginXHTML : IContentXHTML
    {
        public static readonly string LOGO_PATH = Path.Join("..", "Resources", "logo.png").InverseSlash();

        public OriginXHTML()
        {
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
            layout.Body.AddChild(company);
            return layout;
        }
    }
}
