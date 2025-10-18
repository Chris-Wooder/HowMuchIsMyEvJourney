# HowMuchIsMyEvJourney
EV Journey calculator

## Overview
A web application to calculate costs and metrics for EV (Electric Vehicle) journeys, built with .NET 9 and Blazor WebAssembly.

## Project Structure
```
HowMuchIsMyEvJourney/
├── src/
│   ├── HowMuchIsMyEvJourney.Api/     # .NET Core Web API backend
│   └── HowMuchIsMyEvJourney.Client/  # Blazor WebAssembly frontend
└── HowMuchIsMyEvJourney.sln          # Solution file
```

## Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later

## Getting Started

### Building the Solution
```bash
dotnet build
```

### Running the API
```bash
cd src/HowMuchIsMyEvJourney.Api
dotnet run
```
The API will be available at `https://localhost:5001` (or `http://localhost:5000`)

### Running the Blazor Client
```bash
cd src/HowMuchIsMyEvJourney.Client
dotnet run
```
The Blazor app will be available at `https://localhost:5001` (or `http://localhost:5000`)

### Running Tests
```bash
dotnet test
```

## Development

### API Project
- Built with ASP.NET Core Web API
- Uses minimal APIs
- Includes OpenAPI/Swagger support in development mode
- Sample endpoint: `/weatherforecast`

### Client Project  
- Built with Blazor WebAssembly
- Bootstrap 5 for styling
- Sample pages: Home, Counter, Weather

## License
This project is licensed under the terms specified in the repository.
