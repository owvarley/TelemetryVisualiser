FROM quay.io/influxdb/influxdb:v2.0.2

# Copy and sort startup scripts
RUN mv entrypoint.sh entrypoint_influxdb.sh
COPY scripts/docker-entrypoint.sh entrypoint.sh
RUN chmod +x entrypoint.sh

# Copy in template
COPY template/telemetry_template.yml /home/templates/template.yml

ENTRYPOINT [ "/bin/bash", "entrypoint.sh" ]