using PuppeteerSharp;

namespace HeadlessBrowser
{
    public static class PuppeteerExtension
    {
        public static async Task<string> GetPropertyStringValueAsync(this IElementHandle element, string propertyName)
        {
            var property = await element.GetPropertyAsync(propertyName);
            var propertyValue = property.RemoteObject.Value;
            return propertyValue.ToString();
        }

        public static async Task<string> GetInnerTextAsync(this IElementHandle element)
        {
            return await element.GetPropertyStringValueAsync("innerText");
        }

        public static async Task<string> GetOnclickScript(this IPage page, string selector)
        {
            string onclickValue = await page.EvaluateFunctionAsync<string>(
                """
                (selector) => {
                    const element = document.querySelector(selector);
                    return element?.getAttribute('onclick');
                }
                """
                , selector);
            return onclickValue;
        }
    }
}
