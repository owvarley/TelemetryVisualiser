#!/bin/bash
set -m # allow job control

echo Checking password and token have been set
if [ "$INFLUXDB_PASSWORD" = "" ]; then
    echo "Error : No user password supplied via INFLUXDB_PASSWORD environment variable. Unable to proceed."
    exit 1
fi

if [ "$INFLUXDB_TOKEN" = "" ]; then
    echo "Error : No access token defined via INFLUXDB_TOKEN environment variable. Unable to proceed."
    exit 1
fi

if [ "$INFLUXDB_PASSWORD" = "defaultpassword_changeme" ]; then
    echo "** SECURITY WARNING ** : The default user password is being used, this should be changed to prevent compromise."
fi

if [ "$INFLUXDB_TOKEN" = "defaulttoken_changeme" ]; then
    echo "** SECURITY WARNING ** : The default access token is being used, this should be changed to prevent compromise."
fi


echo Booting up Influx DB in the background
/usr/bin/influxd &

# Wait for the instance to be up
while true ; do
    result=$(influx ping)

    if [ "$result" = "OK" ]; then
        break
    fi

    echo Waiting for Influx DB to boot...
    sleep 5
done
echo Influx DB booted

# Run the initial setup
echo Running initial influx inital setup
influx setup -o oc \
             -b $INFLUXDB_BUCKET \
             -u $INFLUXDB_USER \
             -p $INFLUXDB_PASSWORD \
             -t $INFLUXDB_TOKEN \
             -f

echo Initial Setup complete

echo Apply the Telemetry template
influx apply -o $INFLUXDB_ORG -f /home/templates/template.yml --force true

echo Bring Influx to foreground
fg 1
