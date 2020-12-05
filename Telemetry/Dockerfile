FROM alpine:3.12.1

RUN adduser --disabled-password tele_user
USER tele_user
COPY telemetry /usr/bin/

ENTRYPOINT ["telemetry"]