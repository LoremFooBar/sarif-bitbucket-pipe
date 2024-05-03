using System.Web;
using LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.CodeAnnotations;
using LoremFooBar.SarifBitbucketPipe.Model.Bitbucket.Report;

namespace LoremFooBar.SarifBitbucketPipe.BitbucketApiClient;

public partial class BitbucketClient
{
    public async Task CreateReportAsync(PipelineReport report, IEnumerable<Annotation> annotations)
    {
        string serializedReport = Serialize(report);

        _logger.Debug("Sending report: {Report}", serializedReport);

        var response = await _httpClient.PutAsync(
            $"commit/{_bitbucketEnvironmentInfo.CommitHash}/reports/{HttpUtility.UrlEncode(report.ExternalId)}",
            CreateStringContent(serializedReport));

        await VerifyResponseAsync(response);

        await CreateReportAnnotationsAsync(report, annotations);
    }

    private async Task CreateReportAnnotationsAsync(PipelineReport report, IEnumerable<Annotation> annotations)
    {
        const int maxAnnotations = 1000;
        const int maxAnnotationsPerRequest = 100;
        int numOfAnnotationsUploaded = 0;
        var annotationsList = annotations.ToList(); // avoid multiple enumerations

        _logger.Debug("Total annotations: {TotalAnnotations}", annotationsList.Count);

        while (numOfAnnotationsUploaded < annotationsList.Count &&
               numOfAnnotationsUploaded + maxAnnotationsPerRequest <= maxAnnotations) {
            var annotationsToUpload =
                annotationsList.Skip(numOfAnnotationsUploaded).Take(maxAnnotationsPerRequest).ToList();

            string serializedAnnotations = Serialize(annotationsToUpload);

            _logger.Debug("POSTing {TotalAnnotations} annotation(s), starting with location {AnnotationsStart}",
                annotationsToUpload.Count.ToString(), numOfAnnotationsUploaded);
            _logger.Debug("Annotations in request: {Annotations}", serializedAnnotations);

            var response = await _httpClient.PostAsync(
                $"commit/{_bitbucketEnvironmentInfo.CommitHash}/" +
                $"reports/{HttpUtility.UrlEncode(report.ExternalId)}/annotations",
                CreateStringContent(serializedAnnotations));

            await VerifyResponseAsync(response);

            numOfAnnotationsUploaded += annotationsToUpload.Count;
        }
    }
}
