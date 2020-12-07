# TelemetryVisualiser
This project allows for telemetry from a ground station to be processed, stored and available for display.

## Components
The project is comprised of three distinct services:
1. Telemetry Generator
2. Telemetry Receiver
3. Telemetry Visualiser

## Telemetry Generator
The Telemetry Generator is a TCP Server that transmits telemetry on two satellites using two different encoding methods. Each satellite transmits its telemetry at a rate of approximately **2 Hz**. The telemetry transmitted consists of a timestamp, an ID indicating the type of telemetry and a value. From observation, each satellite only transmits telemetry for a single, unique ID. The two encoding methods used are **UTF-8 String** and **LE Binary**. Each satellite's data is transmited on a different port with the default ports in use being **8000** for the **UTF-8 String** and **8001** for the **LE Binary**.

The Telemetry Generator can be found within the **Telemetry** folder and consists of a single linux binary and an associated Dockerfile. The docker-compose file in the root project directory will run up the generator using the default ports of 8000 and 8001.

## Telemetry Receiver
The Telemetry Receiver handles the receiving of telemetry data from the **Telemetry Generator**, decoding it and then transmitting it onwards for storage within a database. The project has been written in C# using .NET Core 3.1 and is designed to be run within a container as a service. 

### Start-up Arguments
The Telemetry Receiver can be configured to receive either **UTF-8 String** Encoding or **LE Binary** Encoding. The receiver encoding to use is set by argument passed to the container at runtime. To configure for **UTF-8 String** on port **8000** the container should be started with a command **string** for **LE Binary** on port **8001** the container should be started with the command **binary**. 

For example, to run up the **Telemetry Receiver** for **UTF-8 String** encoding as an independent container type:
```docker run -d -p 8000:8000 --env-file env_vars oc/telemetryreceiver string```
For **LE Binary** type:
```docker run -d -p 8001:8001 --env-file env_vars oc/telemetryreceiver binary```

### Configuring the DB Driver
The project has been designed to use **Influx OSS DB** as the storage and visualisation engine. This integration is achieved through the **cInfluxDriver** class within the **Decoder**. The connection details for Influx can be manipulated via environmental variable and the required variables can be found in the **env_vars.env** file in the root of the project. This includes the following settings:

- INFLUXDB_HOST = Host name to use for connecting to the Influx OSS DB instance
- INFLUXDB_PORT = Port for the Influx OSS DB instance
- INFLUXDB_ORG = Organisation to create and use within Influx
- INFLUXDB_BUCKET = Data bucket to use for storing telemetry
- INFLUXDB_USER = User name to create and use within Influx
- INFLUXDB_PASSWORD = Password for the username
- INFLUXDB_TOKEN = Authorisation Token for use in Influx

## Telemetry Visualiser
The Telemetry Visualiser handles the storage and display of the telemetry data that's received, decoded and transmitted onwards by a **Telemetry Receiver**. It provides a thin wrapper around the Influx OSS DB official Docker image (v2.0.2). The wrapper handles the initialisation and application of the appropriate template to ensure the most efficient display of telemetry within the solution.

### Usage
To view the telemetry, open a web browser and go to ```http:\\localhost:8086``` and then login using the credentials supplied in the **env_vars.env** file. Once logged in, click **Boards** and then **Satellite Telemetry** to view a display of the data being transmitted from the **Telemetry Generator**. By default, the dashboard will be paused so be sure to select an appropriate refresh rate (Suggested is 5s) from the top right options to view a live display of data from the satellites.

## Running the Project
The project has been configured to be run using **Docker Compose**. To run it, follow the steps below:

1. Open an appropriate command line client and clone the Git repository to your local machine
2. Edit the **env_vars.env** file to supply a **INFLUXDB_PASSWORD** and **INFLUXDB_TOKEN**
3. Type **docker-compose up --build** from the root directory
4. Open a web browser and navigate to **http:\\localhost:8086** then log in using the username and password from **env_vars.env**
5. Click the **boards** option of the side-bar, select **Satellite Telemetry** then select **5s** from the Refresh rate drop-down.
