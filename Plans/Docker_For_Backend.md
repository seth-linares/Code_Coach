# Setting Up the ASP.NET Core Application

1. **Dockerize the ASP.NET Core Application**

Create a `Dockerfile` in the root of your ASP.NET Core project with the following content:

```Dockerfile
# Use the ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MyApp/MyApp.csproj", "./"]
RUN dotnet restore "MyApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "MyApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyApp.csproj" -c Release -o /app/publish

# Copy the published app to the runtime container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

This `Dockerfile` does the following:
- Uses the .NET SDK to restore dependencies, build, and publish the application.
- Copies the published application into a new image based on the ASP.NET Core runtime for a smaller, production-ready image.

2. **Build the Docker Image**

Navigate to the directory containing your `Dockerfile` and run:

```bash
docker build -t myapp .
```

This command builds the Docker image for your application with the tag `myapp`.

### Setting Up SQL Server in Docker

1. **Running SQL Server in a Docker Container**

You don’t need a custom `Dockerfile` for SQL Server, as you can use the official image directly. To run SQL Server and ensure data persistence using an EBS volume, follow these steps:

- **Attach an EBS Volume**: Attach an EBS volume to your EC2 instance. Let’s assume it’s mounted at `/mnt/my-ebs-volume`.

- **Run SQL Server Container**: Use the following command to start SQL Server, mapping a directory from your EBS volume to the container’s data directory:

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrong!Passw0rd' \
   -p 1433:1433 --name sqlserver \
   -v /mnt/my-ebs-volume/sql_data:/var/opt/mssql \
   -d mcr.microsoft.com/mssql/server:2019-latest
```

This command does the following:
- Accepts the EULA and sets a strong password for the `SA` account.
- Maps port 1433 to the host.
- Mounts `/mnt/my-ebs-volume/sql_data` from the host to `/var/opt/mssql` in the container, ensuring data persists across container restarts or migrations.

### Docker Compose for Orchestrating Containers

Create a `docker-compose.yml` file to manage both the application and SQL Server containers together:

```yaml
version: '3.8'
services:
  webapp:
    build: .
    ports:
      - "80:80"
    depends_on:
      - db

  db:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      SA_PASSWORD: "YourStrong!Passw0rd"
      ACCEPT_EULA: "Y"
    volumes:
      - /mnt/my-ebs-volume/sql_data:/var/opt/mssql
    ports:
      - "1433:1433"
```

This configuration:
- Builds and runs the ASP.NET Core application container from the local `Dockerfile`.
- Runs the SQL Server container with the specified environment variables and volume for data persistence.

### Running Your Containers with Docker Compose

In the directory containing your `docker-compose.yml` file, run:

```bash
docker-compose up -d
```

This command starts both the application and SQL Server containers. The `-d` flag runs them in detached mode.




## Managing Application Code for ASP.NET Core Backend

When you build a Docker image for your ASP.NET Core application using a `Dockerfile`, the application code is bundled into the image at build time. The `COPY` command in the Dockerfile copies your application code into the image. This means the running container created from this image inherently contains your application code. Here’s the relevant part from the Dockerfile example:

```Dockerfile
COPY . .
RUN dotnet publish "MyApp.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
```

This sequence copies your application code into the Docker image and then publishes your app. When you start a container from this image, the container runs your application code as bundled at build time.

### Managing Persistent Data for SQL Server

For the SQL Server database, persistent data (like databases, logs) should not be stored inside the container's writable layer, because you’ll lose this data when the container is deleted. Instead, Docker volumes are used to persist this data on the host machine outside the container's lifecycle. 

When you run the SQL Server container, you specify a volume that maps a directory on the host (EBS volume in your case) to the container. This tells SQL Server, "Hey, use this directory for your data files."

Here’s how you can modify the command to run SQL Server with a Docker volume for data persistence:

```shell
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrong!Passw0rd' \
   -p 1433:1433 --name sqlserver \
   -v sql_data:/var/opt/mssql \
   -d mcr.microsoft.com/mssql/server:2019-latest
```

In this command, `-v sql_data:/var/opt/mssql` creates a volume named `sql_data` (if it doesn’t already exist) and mounts it to `/var/opt/mssql` inside the container, which is the default location SQL Server uses for its data files.

### Incorporating Into Docker Compose

You can define both the application service and the SQL Server service with volumes in a `docker-compose.yml` file like so:

```yaml
version: '3.8'
services:
  webapp:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "80:80"

  db:
    image: "mcr.microsoft.com/mssql/server:2019-latest"
    environment:
      SA_PASSWORD: "YourStrong!Passw0rd"
      ACCEPT_EULA: "Y"
    volumes:
      - sql_data:/var/opt/mssql
    ports:
      - "1433:1433"

volumes:
  sql_data:
```

This `docker-compose.yml` file tells Docker Compose to:
- Build and run your ASP.NET Core application container.
- Run a SQL Server container using the official image, with a volume named `sql_data` for persisting database files.
- Automatically create the `sql_data` volume if it doesn't exist.

### EBS Volumes as Docker Volumes

When you're running Docker on an EC2 instance and using an EBS volume for persistent storage:
- First, attach the EBS volume to your EC2 instance.
- Mount the EBS volume to a directory on your EC2 instance.
- When specifying volumes in your Docker run command or Docker Compose file, you map the host directory (where the EBS volume is mounted) to the appropriate directory in your container.

This setup ensures that your SQL Server database files are stored on the EBS volume, providing persistence beyond the life of the container. Similarly, while the application code is bundled into the Docker image at build time and doesn't require external volume for running, you can still use volumes for logs or any data you wish to persist or share outside the container.

### Conclusion

This setup ensures that your ASP.NET Core application is running in a Docker container, with SQL Server in another container, both orchestrated by Docker Compose. The SQL Server container uses a volume mounted from an AWS EBS volume for data persistence, protecting your data across container restarts and migrations.

Remember to replace placeholders like `YourStrong!Passw0rd` with your actual data and adjust paths as needed for your environment. This guide serves as a starting point for developing and deploying containerized applications with persistence on AWS.