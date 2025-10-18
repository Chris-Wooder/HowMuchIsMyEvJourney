# HowMuchIsMyEvJourney

Azure Function App for calculating the cost of electric vehicle (EV) journeys based on distance, electricity rate, and vehicle efficiency.

## Features

- **Calculate Journey Cost**: HTTP POST endpoint to calculate the cost of an EV journey
- **Health Check**: HTTP GET endpoint to verify service health

## API Endpoints

### 1. Calculate Journey Cost
**Endpoint**: `POST /api/journey/calculate`

**Request Body**:
```json
{
  "distanceKm": 100,
  "electricityRatePerKwh": 0.15,
  "vehicleEfficiencyKwhPer100Km": 18,
  "currency": "USD"
}
```

**Response**:
```json
{
  "distanceKm": 100,
  "electricityRatePerKwh": 0.15,
  "vehicleEfficiencyKwhPer100Km": 18,
  "energyUsedKwh": 18,
  "totalCost": 2.7,
  "currency": "USD"
}
```

### 2. Health Check
**Endpoint**: `GET /api/health`

**Response**:
```json
{
  "status": "Healthy",
  "service": "EV Journey Calculator",
  "timestamp": "2025-10-18T10:25:00Z"
}
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Azure Functions Core Tools (for local development)

### Running Locally

1. Navigate to the project directory:
```bash
cd src/EvJourneyCalculator
```

2. Run the function app:
```bash
dotnet run
```

Or using Azure Functions Core Tools:
```bash
func start
```

The function app will start on `http://localhost:7071`

### Building the Project

```bash
cd src/EvJourneyCalculator
dotnet build
```

## Deployment

This Azure Function App can be deployed to Azure using:
- Azure CLI
- Visual Studio
- VS Code with Azure Functions extension
- GitHub Actions (CI/CD)

## Project Structure

```
src/EvJourneyCalculator/
├── CalculateJourneyCost.cs   # Main HTTP trigger function
├── HealthCheck.cs             # Health check endpoint
├── Program.cs                 # Application entry point
├── host.json                  # Azure Functions host configuration
├── local.settings.json        # Local development settings
└── EvJourneyCalculator.csproj # Project file
```
