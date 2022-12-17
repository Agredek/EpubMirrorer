using AngleSharp;
using VersOne.Epub;

namespace EpubMirrorer.Strategies;

public class WordsMirroringStrategy : IMirroringStrategy
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
            var words = node.TextContent.Split(' ');
            if (words is not {Length: > 0})
                continue;

            for (var index = 0; index < words.Length; index++)
            {
                var word = words[index];
                var array = word.ToCharArray();
                Array.Reverse(array);
                words[index] = new string(array);
            }

            node.TextContent = string.Join(' ', words);
        }

        var html = document.ToHtml();
        textContentFile.SetContent(contentStream, html);
    }
}