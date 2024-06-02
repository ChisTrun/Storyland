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
            var css = new XHTMLElement("style");
            css.AddText("""
            .cover-container {
                display: flex;
                flex-direction: column;
                justify-content: center;
                align-items: center;
                height: 100%;
                /* Adjust this if needed */
                overflow: hidden;
                margin-top: 5%;
            }

            .cover-container>img {
                height: 50vh;
                max-width: 50vw;
            }

            .copyright {
                font-size: 0.8em;
                font-family: sans-serif;
            }
            """);
            layout.Head.AddChild(css);

            var company = new XHTMLElement("div", new() { { "class", "cover-container" } }, new() {
                new XHTMLElementInline("img", new Dictionary<string, string>()
                {
                    {"alt", "StoryLand" },
                    {"src",  LOGO_PATH }
                }),
                new XHTMLElement("h2", null, null, "StoryLand"),
                new XHTMLElement("p", new(){{"class", "copyright"}}, null, @"Copyright © 2024, Group15, All rights reserved.")
            });
            layout.Body.AddChild(company);
            return layout;
        }
    }
}
