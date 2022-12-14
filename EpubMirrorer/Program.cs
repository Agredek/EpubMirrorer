using HtmlAgilityPack;
using VersOne.Epub;

namespace EpubMirrorer;

internal static class Program
{
    private static void Main(string[] args)
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

        var directoryName = Path.GetDirectoryName(filePath);
        if (directoryName == null)
        {
            Console.WriteLine($"Directory '{directoryName}' doesn't exist!");
            return;
        }

        var fileName = Path.GetFileNameWithoutExtension(filePath);
        fileName += "(reversed)";
        
        var extension = Path.GetExtension(filePath);
        var newFilePath = Path.Combine(directoryName, $"{fileName}{extension}");
        try
        {
            if (File.Exists(newFilePath))
                File.Delete(newFilePath);
            File.Copy(filePath, newFilePath);
        }
        catch
        {
            Console.WriteLine("Exception!");
        }
        
        using var book = EpubReader.OpenBook(filePath);
        foreach (var textContentFile in book.GetReadingOrder())
            ReverseContentText(textContentFile);
    }

    private static void ReverseContentText(EpubTextContentFileRef textContentFile)
    {
        HtmlDocument htmlDocument = new();
        htmlDocument.LoadHtml(textContentFile.ReadContent());
        var textNodes = htmlDocument.DocumentNode.SelectNodes("//p/text()");
        if (textNodes == null)
            return;
        
        foreach (var node in textNodes)
        {
            if (node is not HtmlTextNode {Text.Length: > 1})
                continue;

            var textNode = (HtmlTextNode) node.CloneNode(true);
            var array = textNode.Text.ToCharArray();
            Array.Reverse(array);
            textNode.Text = new string(array);
            htmlDocument.DocumentNode.ReplaceChild(textNode, node);
        }

        var filePathInEpubArchive = textContentFile.FilePathInEpubArchive;
        using var writer = new StringWriter();
        htmlDocument.Save(filePathInEpubArchive);
    }
}