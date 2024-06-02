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
            layout.Head.AddChild(new XHTMLElement("style", null, null, """
                .container {
                    margin: 0 auto;
                    padding: 2%;
                }

                .container-col {
                    margin-bottom: 1em;
                    display: flex;
                    justify-content: space-between;
                    align-items: flex-start;
                }

                .column {
                    padding: 0 2%;
                }

                .column-left {
                    flex: 0.8;
                }

                .column-left img {
                    display: block;
                    margin-top: 1em;
                    margin-left: auto;
                    margin-bottom: auto;
                    margin-right: auto;
                    width: 90%;
                    max-height: 80vh;
                }

                .column-right {
                    flex: 1.2;
                }

                p,
                h1 {
                    margin-top: 0.5em;
                    margin-bottom: 0.5em;
                    text-indent: unset;
                    line-height: 1.5em;
                    text-align: start;
                    font-family: Arial, Helvetica, sans-serif;
                }

                h1 {
                    margin-top: 0.2em;
                    margin-bottom: 1.5em;
                }
                """));

            XHTMLElement PTagDetailGen(string title, string value)
            {
                return new XHTMLElement("p", null, new()
                {
                    new XHTMLElement("b", null, null, title)
                }, value);
            }
            var bodyContent = new XHTMLElement("div", new() { { "class", "container" } }, new()
            {
                new XHTMLElement("div", new (){{"class","container-col"} }, new(){
                    new XHTMLElement("div", new(){{"class", "column column-left" } }, new()
                    {
                        new XHTMLElementInline("img", new(){ { "alt", "Cover" }, { "src", _storyImagePath } })
                    }),
                new XHTMLElement ("div", new(){{"class", "column column-right" } }, new()
                    {
                        new XHTMLElement("h1", text: _storyTitle),
                        PTagDetailGen("Author: ", _authorName),
                        PTagDetailGen("Categories: ", _storyCategories),
                        PTagDetailGen("Status: ", _storyStatus),
                    }),
                }),
                PTagDetailGen("Description: ", _storyDescription)
            });
            layout.Body.AddChild(bodyContent);
            return layout;
        }
    }
}
