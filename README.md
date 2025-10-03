# Common Address API

## Overview

The Common Address API is an ASP.NET Core web application designed for address validation using the HERE Platform geocode API. It provides endpoints for validating full addresses, postal codes, and location subqueries. The project is built with .NET 9, containerized via Docker for easy deployment, and includes OpenAPI documentation for API exploration. It supports custom JSON conversion for enum-keyed dictionaries and asynchronous validation workflows.

- **Language**: C# (.NET 9)
- **Framework**: ASP.NET Core
- **Containerization**: Docker
- **API Documentation**: OpenAPI with Swagger UI
- **License**: [Specify license, e.g., MIT] (update as needed)

## Features

- Asynchronous validation of full user addresses using the HERE Platform.
- Validation of US postal codes (5 or 9 digits).
- Location validation via subqueries, supporting both dictionary-based and list-based inputs.
    - SubqueryTypes
    >```C#
    >public enum SubqueryType
    >{
    >    country,
    >    state,
    >    county,
    >    city,
    >    district,
    >    street,
    >    houseNumber,
    >    postalCode
    >}
    >```
- Custom JSON converter for handling enum-keyed dictionaries (e.g., for subquery types like country, state, postal code).
- Configurable endpoints and base URIs via `appsettings.json`.
- Dockerized setup for consistent local and production environments.

## Prerequisites

- **Docker**: Required for building and running the container. Download Docker Desktop from [https://www.docker.com/products/docker-desktop/](https://www.docker.com/products/docker-desktop/).
- **Windows**: The provided BAT script is optimized for Windows.
- **curl**: Available in Windows 10+ for endpoint testing (optional for manual verification).
- **HERE API Key**: A valid API key from the HERE Platform is required for address validation features.

## Installation

### 1. Extract the Development API Key Installer
- The ZIP file `DevApiKey.7zip` contains an installer that sets up a development environment, including configuring the HERE API key as an environment variable (`ADDRESS_API_KEY`).
- Extract and run the installer to prepare your local machine. This ensures the API key is securely set for running the project without hardcoding sensitive information.

### 2. Clone or Download the Repository
If using Git:
```bash
git clone https://github.com/yourusername/common-address-api.git
cd common-address-api
```
Alternatively, download and extract the source code.

### 3. Build and Run Using the Provided Script
- The `Run Docker Container.bat` script automates the process:
  - Checks for Docker installation and directs you to download if missing.
  - Stops and removes any existing container or image tagged `common-address-api`.
  - Builds a new Docker image from the `Dockerfile`.
  - Runs the container, mapping host port 8080 to container port 80.
  - Waits for the API to start and performs basic tests on endpoints.
  - Opens the Swagger UI in your default browser.
- Run the script:
  ```bat:disable-run
  "Run Docker Container.bat"
  ```

### 4. Manual Setup (Alternative)
- Build the Docker image:
  ```bash
  docker build -t common-address-api .
  ```
- Run the container:
  ```bash
  docker run -d -p 8080:80 --name common-address-api common-address-api
  ```
- Verify the API is running by accessing `http://localhost:8080/swagger/index.html`.

## Usage

### API Endpoints
Access the API at `http://localhost:8080`. Use the Swagger UI at `http://localhost:8080/swagger/index.html` for interactive documentation and testing.

- **POST /ValidateUserAddress/ValidateAddress**
  - Validates a full user address using the HERE Platform.
  - Request body: `UserAddress` object.
  - Response: `200 OK` with validated `RootObject` or `404 Not Found`.
  - expected document format
    > UserAddress
    > ```json
    > {
    >     "street": "string", //required
    >     "city": "string", //required
    >     "state": "string", //required
    >     "zipCode": "string", //required
    >     "country": "string" //required
    > }
    > ```

- **POST /ValidateUserAddress/{postalCode}**
  - Validates a US postal code (5 or 9 digits).
  - Path parameter: `postalCode` (e.g., `12345` or `123456789`).
  - Response: `200 OK` if valid, `400 Bad Request` with error details if invalid.
  - `/ValidateUserAddress/12345[6789]`

- **POST /ValidateUserAddress/AddressSubquery/Dictionary**
  - Validates location using a list of subquery dictionaries.
  - Request body: `Subquery`
    - Subqery Object
    >`Dictionary<SubqueryType, string>`
  - Response: `200 OK` if valid, `400 Bad Request` with error details if invalid.
  - expected document format
    >Subquery
    >```json
    >{
    >   "city": "Chicago",
    >}
    >```
- **POST /ValidateUserAddress/AddressSubquery/List**
  - Validates location using a structured `SubqueryList` object.
  - Request body: `SubqueryList`.
  - Response: `200 OK` if valid, `400 Bad Request` with error details if invalid.
  - expected document
  >SubqueryList
  >```json
  >{
  >     "items":
  >          [
  >              {
  >                 "subquery": 
  >                     {"state": "ga"}
  >              },
  >              {
  >                 "subquery":
  >                     {"postalCode": "30082"}
  >              }
  >          ]
  >};
  >```

### Configuration
- Update `appsettings.json` for HERE API settings and Kestrel endpoints.
- Example:
  ```json
  {
    "Here": {
      "ApiKey": "TEST KEY",
      "BaseUri": "https://geocode.search.hereapi.com/v1",
      "GeocodeEndpoint": "/geocode"
    },
    "Kestrel": {
      "Endpoints": {
        "Http": {
          "Url": "http://0.0.0.0:80"
        }
      }
    }
  }
  ```
- The API key is loaded from the `ADDRESS_API_KEY` environment variable for security.

## Development

### Project Structure
- `Controllers/`: API controllers (e.g., `ValidateUserAddress.cs`).
- `Enumerators/`: Enums for subquery types (e.g., `SubqueryType.cs`).
- `Helper/`: Custom JSON converters (e.g., `EnumKeyDictionaryConverter.cs`).
- `Work/`: Validation logic (e.g., `HerePlatformValidator.cs`).
- `Program.cs`: Application entry point and middleware configuration (OpenAPI, Swagger UI, controllers).
- `appsettings.json`: Configuration settings.
- `Dockerfile`: Multi-stage Docker build for .NET 9 SDK and runtime.
- `.dockerignore`: Excludes build artifacts (e.g., `bin/`, `obj/`).
- `Run Docker Container.bat`: Automation script for Docker setup and testing.

### Dependencies
- ASP.NET Core for web framework.
- Newtonsoft.Json for JSON handling.
- NSwag for OpenAPI/Swagger documentation.

### Running Tests
- Use the `Run Docker Container.bat` script for automated endpoint tests via `curl`.
- For custom tests, use tools like Postman or the Swagger UI to send requests to the validation endpoints.

## Contributing
1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/awesome-feature`).
3. Commit changes (`git commit -m "Add awesome feature"`).
4. Push to the branch (`git push origin feature/awesome-feature`).
5. Open a Pull Request.

## License
 [MIT License](https://github.com/robdesautel/ValidateAddress/blob/main/LICENSE)

## Acknowledgments
AI Tools
- [Copilot](https://m365.cloud.microsoft/)
    - read log reports
    - setup docker environment steps
    - identify best practices for api key secrets for dev
    - troubleshoot json document
    - validate dictionary with enum comparison
- [Grok](https://grok.com/)
    - QA for Copilot Dockerfile
    - troubleshooting docker image
    - troubleshooting Swagger UI
    - this readme doc
    - developing bat file for automated launces

## Contact
[Robert DeSautel](https://github.com/robdesautel/ValidateAddress/issues/new)
```