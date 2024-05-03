param (
    $Version
)

docker build --rm -t loremfoobar/sarif-bitbucket-pipe:$Version .
