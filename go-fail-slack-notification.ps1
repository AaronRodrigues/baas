param([string]$channel)
$GO_PIPELINE_NAME=$env:GO_PIPELINE_NAME
$GO_PIPELINE_COUNTER=$env:GO_PIPELINE_COUNTER
$GO_STAGE_NAME=$env:GO_STAGE_NAME
$GO_STAGE_COUNTER=$env:GO_STAGE_COUNTER
$GO_JOB_NAME=$env:GO_JOB_NAME
$SLACK_WEBHOOK_URL=$env:SLACK_WEBHOOK_URL
$message="energy.quoting-adapter.build failed"
function Post-Json($uri, $json)
{
    Write-Host "Posting" $json "to Slack API" $uri
    $webRequest = [System.Net.WebRequest]::Create($uri)
    $encodedContent = [System.Text.Encoding]::UTF8.GetBytes($json)
    $webRequest.Method = "post"
    $webRequest.ContentType = "application/json"
    $webRequest.Accept = "application/json"
    $webRequest.ContentLength = $encodedContent.length
    $requestStream = $webRequest.GetRequestStream()
    $requestStream.Write($encodedContent, 0, $encodedContent.length)
    $requestStream.Close()
    try
    {
      #[System.Net.HttpWebResponse] $response = [System.Net.HttpWebResponse] $webRequest.GetResponse()
      $response = $webRequest.GetResponse()
    }
    catch
    {
      $response = $Error[0].Exception.InnerException.Response; 
      Throw "Exception occurred in $($MyInvocation.MyCommand): `n$($_.Exception.Message)"
    }
    $reader = [IO.StreamReader] $response.GetResponseStream()  
    $output = $reader.ReadToEnd()  
    $reader.Close()  
    $response.Close()
    Write-Host $output 
}
$go_link = "http://ci.internal.comparethemarket.local:8153/go/tab/build/detail/" + $GO_PIPELINE_NAME + "/" + $GO_PIPELINE_COUNTER + "/" + $GO_STAGE_NAME + "/" + $GO_STAGE_COUNTER + "/" + $GO_JOB_NAME
$json = '{"channel": "#' + $channel + '", "icon_emoji": ":goci:", "username": "GO", "mrkdwn": true, "text": "<!here> ' + $message + '\n<' + $go_link + '|View more details in GO>", "attachments": [ { "color": "danger", "fields": [ {"title": "pipeline", "value": "' + $GO_PIPELINE_NAME + '", "short": false}, {"title": "stage", "value": "' + $GO_STAGE_NAME + '", "short": false}, {"title": "job", "value": "' + $GO_JOB_NAME + '", "short": false} ] } ]}'
Post-Json $SLACK_WEBHOOK_URL $json