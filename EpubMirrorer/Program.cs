using AngleSharp;
using VersOne.Epub;

namespace EpubMirrorer;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length <= 0)
        {
            Console.WriteLine("Invalid number of parameters. For help call program with parameter 'h' or '-h'.");
            return;
        }

        var filePath = args[0];
        if (!Path.IsPathRooted(filePath))
        {
            Console.WriteLine("Provided invalid path!");
            return;
        }

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File under '{filePath}' doesn't exist!");
            return;
        }

        var reversedFilePath = FileHelper.CreateReversedFile(filePath);
        if (reversedFilePath is not {Length: > 0} || !File.Exists(reversedFilePath))
            return;

        using var context = BrowsingContext.New();
        using var book = await EpubReader.OpenBookAsync(reversedFilePath);
        foreach (var textContentFile in await book.GetReadingOrderAsync())
            await ReverseContentText(context, textContentFile);
    }

    private static async Task ReverseContentText(IBrowsingContext context, EpubTextContentFileRef textContentFile)
    {
        await using var contentStream = textContentFile.GetContentStream();
        var document = await context.OpenAsync(req => req.Content(new StreamReader(contentStream).ReadToEnd()));
        var textNodes = document.QuerySelectorAll("p");
        if (textNodes == null)
            return;

        foreach (var node in textNodes)
        {
            var array = node.TextContent.ToCharArray();
            if (array is not {Length: > 0})
                continue;

            Array.Reverse(array);
            node.TextContent = new string(array);
        }
        
        var html = document.ToHtml();
        textContentFile.SetContent(contentStream, html);
    }
}