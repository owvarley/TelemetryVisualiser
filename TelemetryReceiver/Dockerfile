FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine As build
WORKDIR /home/
ARG version=0.0.1

COPY ./Receiver/ ./Receiver
COPY ./ReceiverTest/ ./ReceiverTest
COPY TelemetryReceiver.sln ./

RUN dotnet restore TelemetryReceiver.sln

RUN dotnet tool install dotnet-reportgenerator-globaltool --tool-path /tools
RUN dotnet test --results-directory /testresults --logger "trx;LogFileName=test_results.xml" /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=/testresults/coverage/ /p:Exclude="[xunit.*]*%2c[StackExchange.*]*" ./ReceiverTest/ReceiverTest.csproj

# Build and publish
RUN dotnet publish Receiver/Receiver.csproj -c Release -o out

# Swap to the runtime image to reduce overall image size
FROM mcr.microsoft.com/dotnet/runtime:3.1-alpine
WORKDIR /home
COPY --from=build /home/Receiver/bin .
ENTRYPOINT [ "/home/Release/netcoreapp3.1/Receiver" ]