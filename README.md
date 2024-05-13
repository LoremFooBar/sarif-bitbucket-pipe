﻿# Bitbucket Pipelines Pipe: SARIF Report

Create a report with annotations from a [SARIF](https://sarifweb.azurewebsites.net/)
file, and a corresponding build status with the status of the report.

## YAML Definition

Add the following snippet to the script section of your `bitbucket-pipelines.yml` file:

```yaml
script:
  - pipe: docker://loremfoobar/sarif-bitbucket-pipe:0.1.0
    variables:
      SARIF_FILE_PATH: "<string>"
      # BITBUCKET_USERNAME: "<string>" # Optional
      # BITBUCKET_APP_PASSWORD: "<string>" # Optional
      # CREATE_BUILD_STATUS: "<boolean>" # Optional, default "true"
      # INCLUDE_ONLY_ISSUES_IN_DIFF: "<boolean>" # Optional, default "false"
      # FAIL_WHEN_ISSUES_FOUND: "<boolean>"  # Optional, default "false"
      # DEBUG: "<boolean>" # Optional
```

## Variables

| Variable                    | Usage                                                                                                                                                                                                 |
|-----------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| SARIF_FILE_PATH (\*)        | Path to SARIF file, relative to current directory. You can use patterns that are supported by [DirectoryInfo.GetFiles](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo.getfiles). |
| BITBUCKET_USERNAME          | Bitbucket username, required to create build status and to get PR diff. Note that this should be an account name, not the email.                                                                      |
| BITBUCKET_APP_PASSWORD      | Bitbucket app password, required to create build status and to get PR diff.                                                                                                                           |
| CREATE_BUILD_STATUS         | Whether to create a new build status reflecting the results of the report. Default: `true`.                                                                                                           |
| FAIL_WHEN_ISSUES_FOUND      | Whether to fail current build step if any issues found. Default: `false`.                                                                                                                             |
| INCLUDE_ONLY_ISSUES_IN_DIFF | Whether to include only issues found in changes of current PR/commit. Default: `false`.                                                                                                               |
| DEBUG                       | Turn on extra debug information. Default: `false`.                                                                                                                                                    |

_(\*) = required variable._

## Prerequisites

### SARIF File

You need to create the SARIF file for your project before calling the pipe.

### App Password

App password is required for 2 pipe features:

1. Create build status when `CREATE_BUILD_STATUS="true"`. Required permission: Repositories - Read.
2. Get diff (`INCLUDE_ONLY_ISSUES_IN_DIFF="true"` when in PRs). Required permission: Pull requests - Read.

See Atlassian documentation on how
to [generate an app password](https://confluence.atlassian.com/bitbucket/app-passwords-828781300.html).

## Examples

Basic example:

```yaml
script:
  - pipe: docker://loremfoobar/sarif-bitbucket-pipe:0.1.0
    variables:
      SARIF_FILE_PATH: "issues.sarif"
```

With pattern:

```yaml
script:
  - pipe: docker://loremfoobar/sarif-bitbucket-pipe:0.1.0
    variables:
      SARIF_FILE_PATH: "src/*/issues.sarif"
```

With app password (you should use secure variables for username and app
password):

```yaml
script:
  - pipe: docker://loremfoobar/sarif-bitbucket-pipe:0.1.0
    variables:
      SARIF_FILE_PATH: "issues.sarif"
      BITBUCKET_USERNAME: $USERNAME
      BITBUCKET_APP_PASSWORD: $APP_PASSWORD
```

With build status creation disabled:

```yaml
script:
  - pipe: docker://loremfoobar/sarif-bitbucket-pipe:0.1.0
    variables:
      SARIF_FILE_PATH: "issues.sarif"
      BITBUCKET_USERNAME: $USERNAME
      BITBUCKET_APP_PASSWORD: $APP_PASSWORD
      CREATE_BUILD_STATUS: "false"
```

## Support

If you're reporting an issue, please include:

- the version of the pipe
- relevant logs and error messages
- steps to reproduce

## License

[MIT License](LICENSE)
