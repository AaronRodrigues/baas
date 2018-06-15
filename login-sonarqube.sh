#!/bin/bash

STATUS=$(curl -d "login=${SONARQUBE_USERNAME}&password=${SONARQUBE_PASSWORD}" https://sonarqube-security.test.ctmers.io/api/authentication/login --cookie-jar sonarqube.cookies.txt)
echo $STATUS
