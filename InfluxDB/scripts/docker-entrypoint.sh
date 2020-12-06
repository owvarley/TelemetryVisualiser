#!/bin/bash
set -m # allow job control

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