using AngleSharp;
using EpubMirrorer.Strategies;
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
        var strategy = new WordsMirroringStrategy();
        foreach (var textContentFile in await book.GetReadingOrderAsync())
            await strategy.ReverseContentText(context, textContentFile);
    }
}