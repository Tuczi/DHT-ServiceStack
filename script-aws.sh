#!/bin/bash
mkdir dht-server
cd dht-server
aws s3 cp s3://ryba-web-app/dht-aws.tar.gz .
tar xvf dht-aws.tar.gz
screen -dmS app mono bin/Debug/server.exe
