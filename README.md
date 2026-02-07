# Bitbucket Pipelines Pipe: SARIF Report

Create a report with annotations from a [SARIF](https://sarifweb.azurewebsites.net/)
file, and a corresponding build status with the status of the report.

## YAML Definition

Add the following snippet to the script section of your `bitbucket-pipelines.yml` file:

```yaml
script:
  - pipe: docker://loremfoobar/sarif-bitbucket-pipe:0.1.1
    variables:
      SARIF_FILE_PATH: "<string>"
      ACCOUNT_EMAIL: "<string>"
      API_TOKEN: "<string>"
      # CREATE_BUILD_STATUS: "<boolean>" # Optional, default "true"
      # INCLUDE_ONLY_ISSUES_IN_DIFF: "<boolean>" # Optional, default "false"
      # FAIL_WHEN_ISSUES_FOUND: "<boolean>"  # Optional, default "false"
      # DEBUG: "<boolean>" # Optional
```

## Variables

| Variable                    | Usage                                                                                                                                                                                                 |
|-----------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| SARIF_FILE_PATH (\*)        | Path to SARIF file, relative to current directory. You can use patterns that are supported by [DirectoryInfo.GetFiles](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo.getfiles). |
| ACCOUNT_EMAIL (\*)           | Bitbucket username, required to create build status and to get PR diff. Note that this should be your Atlassian account email.                                                                      |
| API_TOKEN (\*)              | Bitbucket API token, required to create build status and to get PR diff.                                                                                                                           |
| CREATE_BUILD_STATUS         | Whether to create a new build status reflecting the results of the report. Default: `true`.                                                                                                           |
| FAIL_WHEN_ISSUES_FOUND      | Whether to fail current build step if any issues found. Default: `false`.                                                                                                                             |
| INCLUDE_ONLY_ISSUES_IN_DIFF | Whether to include only issues found in changes of current PR/commit. Default: `false`.                                                                                                               |
| DEBUG                       | Turn on extra debug information. Default: `false`.                                                                                                                                                    |

_(\*) = required variable._

## Prerequisites

### SARIF File

You need to create the SARIF file for your project before calling the pipe.

### Authentication

An API token and the corresponding Atlassian account email are required for the pipe to work. These are used for the following features:

| Feature               | Required scope               |
|-----------------------|------------------------------|
| Create a report       | `read:repository:bitbucket`  |
| Create a build status | `read:repository:bitbucket`  |
| Get commit diff       | `read:repository:bitbucket`  |
| Get PR diff           | `read:pullrequest:bitbucket` |

See Atlassian documentation on how
to [create an API token](https://support.atlassian.com/bitbucket-cloud/docs/create-an-api-token/).

## Examples

Basic example (both `ACCOUNT_EMAIL` and `API_TOKEN` are required):

```yaml
script:
  - pipe: docker://loremfoobar/sarif-bitbucket-pipe:0.1.1
    variables:
      SARIF_FILE_PATH: "issues.sarif"
      ACCOUNT_EMAIL: $ACCOUNT_EMAIL
      API_TOKEN: $API_TOKEN
```

With pattern:

```yaml
script:
  - pipe: docker://loremfoobar/sarif-bitbucket-pipe:0.1.1
    variables:
      SARIF_FILE_PATH: "src/*/issues.sarif"
      ACCOUNT_EMAIL: $ACCOUNT_EMAIL
      API_TOKEN: $API_TOKEN
```

With failure on issues:

```yaml
script:
  - pipe: docker://loremfoobar/sarif-bitbucket-pipe:0.1.1
    variables:
      SARIF_FILE_PATH: "issues.sarif"
      ACCOUNT_EMAIL: $ACCOUNT_EMAIL
      API_TOKEN: $API_TOKEN
      FAIL_WHEN_ISSUES_FOUND: "true"
```

With build status creation disabled:

```yaml
script:
  - pipe: docker://loremfoobar/sarif-bitbucket-pipe:0.1.1
    variables:
      SARIF_FILE_PATH: "issues.sarif"
      ACCOUNT_EMAIL: $ACCOUNT_EMAIL
      API_TOKEN: $API_TOKEN
      CREATE_BUILD_STATUS: "false"
```

## Support

If you're reporting an issue, please include:

- the version of the pipe
- relevant logs and error messages
- steps to reproduce

## License

[MIT License](LICENSE)
