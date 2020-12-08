FROM quay.io/influxdb/influxdb:v2.0.2

COPY scripts/docker-entrypoint.sh docker-entrypoint.sh
COPY template/telemetry_template.yml /home/templates/template.yml

RUN apt-get update \
 && apt-get install -y dos2unix \
 && dos2unix docker-entrypoint.sh \
 && chmod +x docker-entrypoint.sh \
 && apt-get remove --purge -y dos2unix \
 && rm -rf /var/lib/apt/lists/*

ENTRYPOINT [ "/bin/bash", "docker-entrypoint.sh" ]