namespace EpubMirrorer;

public static class FileHelper
{
    public static string CreateReversedFile(string originalFilePath)
    {
        var directoryName = Path.GetDirectoryName(originalFilePath);
        if (directoryName == null)
        {
            Console.WriteLine($"Directory '{directoryName}' doesn't exist!");
            return string.Empty;
        }

        var fileName = Path.GetFileNameWithoutExtension(originalFilePath);
        fileName += "(reversed)";

        var extension = Path.GetExtension(originalFilePath);
        var newFilePath = Path.Combine(directoryName, $"{fileName}{extension}");
        try
        {
            if (File.Exists(newFilePath))
                File.Delete(newFilePath);
            File.Copy(originalFilePath, newFilePath);
        }
        catch
        {
            Console.WriteLine("Exception!");
            return string.Empty;
        }

        return newFilePath;
    }
}