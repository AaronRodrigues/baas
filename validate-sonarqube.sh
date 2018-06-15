#!/bin/bash

PROJECTSTATUS=$(curl -b sonarqube.cookies.txt https://sonarqube-security.test.ctmers.io//api/qualitygates/project_status?projectKey=CTM.Allspark.ProviderAdapterTemplate)
echo "project status is : " $PROJECTSTATUS

STATUSCODE=$(curl -b sonarqube.cookies.txt https://sonarqube-security.test.ctmers.io//api/qualitygates/project_status?projectKey=CTM.Allspark.ProviderAdapterTemplate | jq '.projectStatus.status')
echo "status code is : "$STATUSCODE
if [ $STATUSCODE != '"OK"' ]; then
    echo "Sonarqube quality criteria not met. Check the dashboard."
    exit 1
fi
exit 0
