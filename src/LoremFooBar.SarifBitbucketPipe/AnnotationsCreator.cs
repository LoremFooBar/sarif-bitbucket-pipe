using LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.CodeAnnotations;
using LoremFooBar.SarifBitbucketPipe.Utils;
using Microsoft.CodeAnalysis.Sarif;

namespace LoremFooBar.SarifBitbucketPipe;

public class AnnotationsCreator
{
    private readonly Uri? _cloneDirUri;

    public AnnotationsCreator(BitbucketEnvironmentInfo environmentInfo)
    {
        Uri.TryCreate("file:///" + environmentInfo.CloneDir, UriKind.Absolute, out _cloneDirUri);
    }

    public IEnumerable<Annotation> CreateAnnotationsFromSarifResults(
        IReadOnlyList<ResultWithRun> results)
    {
        if (results.Count == 0) yield break;

        for (int i = 0; i < results.Count; i++) {
            var result = results[i].Result;
            var run = results[i].Run;
            var physicalLocation = result.Locations.FirstOrDefault()?.PhysicalLocation;

            if (physicalLocation is null) continue;

            string filePath = physicalLocation.ArtifactLocation.Uri.OriginalString;

            if (string.IsNullOrEmpty(filePath)) continue;

            var rule = result.GetRule(run);
            string details = result.Message.Text + (rule.HelpUri == default ? "" : "\n" + rule.HelpUri);
            /*string relativePathPart = GetRelativePathPart();*/
            string pathRelativeToCloneDir = GetPathRelativeToCloneDir(physicalLocation.ArtifactLocation, run);

            yield return new Annotation
            {
                ExternalId = $"issue-{i + 1}",
                AnnotationType = AnnotationType.CodeSmell,
                Path = pathRelativeToCloneDir,
                Line = physicalLocation.Region.StartLine,
                Summary = string.IsNullOrWhiteSpace(rule.ShortDescription.Text)
                    ? rule.FullDescription.Text
                    : rule.ShortDescription.Text,
                Details = details,
                Result = AnnotationResult.Failed,
            };
        }
    }

    private string GetPathRelativeToCloneDir(ArtifactLocation artifactLocation, Run run)
    {
        var resultUri = artifactLocation.Uri;

        if (_cloneDirUri is null) return resultUri.ToString();

        Uri absoluteUri;

        if (resultUri.IsAbsoluteUri)
            absoluteUri = resultUri;
        else {
            if (string.IsNullOrEmpty(artifactLocation.UriBaseId) || run.OriginalUriBaseIds is null)
                return resultUri.ToString();

            bool gotBaseLocation =
                run.OriginalUriBaseIds.TryGetValue(artifactLocation.UriBaseId, out var baseLocation);

            if (!gotBaseLocation || baseLocation is null) return resultUri.ToString();

            absoluteUri = new Uri(baseLocation.Uri, resultUri);
        }

        return absoluteUri.ToString()[(_cloneDirUri.ToString().Length + 1)..];
    }
}
