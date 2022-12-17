using AngleSharp;
using VersOne.Epub;

namespace EpubMirrorer.Strategies;

public interface IMirroringStrategy
{
    Task ReverseContentText(IBrowsingContext context, EpubContentFileRef textContentFile);
}