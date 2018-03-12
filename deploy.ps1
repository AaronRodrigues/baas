param (
  [string]
  $env,
  [string]
  $version = "2.0"
)

$key = "providers-$env/energy/$version/all/all.zip"

if ($key -like '*providers-prod*') { $key = "providers/energy/$version/all/all.zip" }

Write-Host "Pushing all.zip to $key"

Write-S3Object -BucketName ctm-panel -file "build-packaged/all.zip" -key $key