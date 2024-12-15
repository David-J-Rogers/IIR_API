# Azure Function: Get Event by ID

This project is an Azure Functions application that queries an external API to retrieve details about events based on their ID.

---

## Technologies Used

- **Azure Functions** (v4)
- **.NET 8.0**
- **Newtonsoft.Json** for JSON deserialization
- **HttpClient** for API requests

---

## Prerequisites

1. **.NET SDK 8.0 or later**
   ```bash
   brew install dotnet

4. **Azure Functions Core Tools** (v4)  
   ```bash
   brew tap azure/functions
   brew install azure-functions-core-tools@4

---

## Setup and Installation

Follow these steps to set up and run the project locally:

1. **Clone the Repository**
   Clone the GitHub repository to your local machine:
   ```bash
   git clone https://github.com/David-J-Rogers/IIR_API.git
   cd IIR_API
2. **Build the project**
   ```bash
   dotnet restore
   dotnet build
3. **Verify access at link below**
   ```bash
   http://localhost:7071/api/events/{id}

---

## Usage
   ```bash
   curl -X GET "http://localhost:7071/api/events/{ID}"
