namespace LoremFooBar.SarifBitbucketPipe.Model.Diff;

public record AddedLinesInFile(string FilePath, List<int> LinesAdded);
