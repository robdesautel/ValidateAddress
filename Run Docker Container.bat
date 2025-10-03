@echo off
setlocal enabledelayedexpansion

:: Script Name: DockerSetupAndRun.bat
:: Description: Checks for Docker installation, manages existing containers and images, builds and runs a Docker container for the Common Address API, tests endpoints, and opens the Swagger UI in the browser.
:: Assumptions:
:: - Dockerfile is in the current directory.
:: - Image name: common-address-api:latest
:: - Container name: common-address-api
:: - Host port: 8080 mapped to container port 80
:: - Endpoints: /weatherforecast and /swagger/v1/swagger.json
:: - Requires curl (available in Windows 10+)
:: - Runs on Windows (BAT file)

:: Set variables
set IMAGE_NAME=common-address-api
set CONTAINER_NAME=common-address-api
set HOST_PORT=8080
set CONTAINER_PORT=80
set WEATHER_ENDPOINT=http://localhost:%HOST_PORT%/weatherforecast
set SWAGGER_ENDPOINT=http://localhost:%HOST_PORT%/swagger/v1/swagger.json
set SWAGGER_UI=http://localhost:%HOST_PORT%/swagger/index.html
set MAX_WAIT=30  :: Max seconds to wait for container startup

echo [INFO] Starting Docker setup and run process...

:: Step 1: Check if Docker is installed
echo [INFO] Checking if Docker is installed...
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Docker is not installed.
    echo [INFO] Detected OS: Windows
    echo [INFO] Download Docker Desktop from: https://www.docker.com/products/docker-desktop/
    echo [INFO] After installation, rerun this script.
    pause
    exit /b 1
)
echo [INFO] Docker is installed.

:: Step 2: Check if a container with the same name is running and stop it
echo [INFO] Checking for running container '%CONTAINER_NAME%'...
for /f %%i in ('docker ps -q --filter "name=^%CONTAINER_NAME%$" --filter "status=running"') do (
    if "%%i" neq "" (
        echo [INFO] Running container found. Stopping it...
        docker stop %CONTAINER_NAME%
        if %errorlevel% neq 0 (
            echo [ERROR] Failed to stop container.
            pause
            exit /b 1
        )
        echo [INFO] Container stopped.
    )
)

:: Step 3: Check if a container with the same name exists (even if stopped) and remove it
echo [INFO] Checking for existing container '%CONTAINER_NAME%'...
for /f %%i in ('docker ps -aq --filter "name=^%CONTAINER_NAME%$"') do (
    if "%%i" neq "" (
        echo [INFO] Existing container found. Removing it...
        docker rm %CONTAINER_NAME%
        if %errorlevel% neq 0 (
            echo [ERROR] Failed to remove container.
            pause
            exit /b 1
        )
        echo [INFO] Container removed.
    )
)

:: Step 4: Check if the image exists and remove it
echo [INFO] Checking for existing image '%IMAGE_NAME%'...
for /f %%i in ('docker images -q %IMAGE_NAME%') do (
    if "%%i" neq "" (
        echo [INFO] Existing image found. Removing it...
        docker rmi %IMAGE_NAME%
        if %errorlevel% neq 0 (
            echo [ERROR] Failed to remove image.
            pause
            exit /b 1
        )
        echo [INFO] Image removed.
    )
)

:: Step 5: Build the Docker image
echo [INFO] Building Docker image '%IMAGE_NAME%' from Dockerfile...
docker build -t %IMAGE_NAME% .
if %errorlevel% neq 0 (
    echo [ERROR] Failed to build Docker image.
    pause
    exit /b 1
)
echo [INFO] Docker image built successfully.

:: Step 6: Run the Docker container
echo [INFO] Running Docker container '%CONTAINER_NAME%'...
docker run -d -p %HOST_PORT%:%CONTAINER_PORT% --name %CONTAINER_NAME% %IMAGE_NAME%
if %errorlevel% neq 0 (
    echo [ERROR] Failed to run Docker container.
    pause
    exit /b 1
)
echo [INFO] Docker container started.

:: Step 7: Wait for the container to start (up to %MAX_WAIT% seconds)
echo [INFO] Waiting for the API to become available...
set WAIT_COUNT=0
:WAIT_LOOP
timeout /t 1 /nobreak >nul
curl -s -o nul -w "%%{http_code}" %WEATHER_ENDPOINT% > temp_status.txt
set /p STATUS_CODE=<temp_status.txt
del temp_status.txt
if "%STATUS_CODE%"=="200" (
    echo [INFO] API is available.
    goto TESTS
)
set /a WAIT_COUNT+=1
if %WAIT_COUNT% geq %MAX_WAIT% (
    echo [ERROR] Timeout waiting for API to start.
    pause
    exit /b 1
)
goto WAIT_LOOP

:: Step 8: Test the endpoints
:TESTS
echo [INFO] Testing WeatherForecast endpoint...
curl -s %WEATHER_ENDPOINT%
if %errorlevel% neq 0 (
    echo [ERROR] WeatherForecast test failed.
) else (
    echo [INFO] WeatherForecast test successful.
)

echo.
echo [INFO] Testing Swagger endpoint...
curl -s %SWAGGER_ENDPOINT%
if %errorlevel% neq 0 (
    echo [ERROR] Swagger test failed.
) else (
    echo [INFO] Swagger test successful.
)

:: Step 9: Open the web browser to the Swagger UI
echo [INFO] Opening Swagger UI in default browser...
start %SWAGGER_UI%

echo [INFO] Process completed successfully.
pause
endlocal