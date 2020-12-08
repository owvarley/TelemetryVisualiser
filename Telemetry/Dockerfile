FROM alpine:3.12.1

COPY telemetry /
RUN chmod +x telemetry
RUN adduser --disabled-password tele_user
USER tele_user

ENTRYPOINT ["./telemetry"]