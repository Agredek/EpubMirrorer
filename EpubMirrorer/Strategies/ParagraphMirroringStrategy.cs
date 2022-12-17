using AngleSharp;
using VersOne.Epub;

namespace EpubMirrorer.Strategies;

public class ParagraphMirroringStrategy : IMirroringStrategy
{
    public async Task ReverseContentText(IBrowsingContext context, EpubContentFileRef textContentFile)
    {
        await using var contentStream = textContentFile.GetContentStream();
        var document = await context.OpenAsync(req => req.Content(new StreamReader(contentStream).ReadToEnd()));
        var textNodes = document.QuerySelectorAll("p");
        if (textNodes == null)
            return;

        foreach (var node in textNodes)
        {
            var array = node.TextContent.ToCharArray();
            if (array is not {Length: > 1})
                continue;

            Array.Reverse(array);
            node.TextContent = new string(array);
        }

        var html = document.ToHtml();
        textContentFile.SetContent(contentStream, html);
    }
}