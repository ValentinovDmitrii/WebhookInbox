# WebhookInbox

WebhookInbox is a lightweight ASP.NET Core Web API for receiving, storing, inspecting, and deleting webhook events.

The project was created as a practical .NET portfolio project and as a foundation for a reusable webhook inspection service.

## Features

Current Stage 1 functionality includes:

- Receive webhook events through HTTP POST requests
- Generate a unique event ID on the server
- Record the event receive timestamp
- Store events in memory
- List all received events
- Retrieve a single event by ID
- Delete an event by ID
- Validate required input fields
- Return structured API error responses
- Use asynchronous controller and repository contracts
- Use thread-safe in-memory storage
- Unit tests for repository and controller behavior

## Technology Stack

- .NET 10
- ASP.NET Core Web API
- xUnit
- Moq
- OpenAPI
- ConcurrentDictionary for thread-safe in-memory storage

## Project Structure

```text
WebhookInbox/
├── src/
│   └── WebhookInbox.Api/
│       ├── Controllers/
│       ├── Models/
│       ├── Repositories/
│       ├── Program.cs
│       └── WebhookInbox.Api.csproj
│
├── tests/
│   └── WebhookInbox.Api.Tests/
│       ├── Controllers/
│       ├── Repositories/
│       └── WebhookInbox.Api.Tests.csproj
│
└── WebhookInbox.slnx
```

## Running Locally

### Prerequisites

- .NET 10 SDK

### Restore dependencies

```bash
dotnet restore
```

### Build the solution

```bash
dotnet build
```

### Run the API

```bash
dotnet run --project src/WebhookInbox.Api
```

The local URLs are defined in:

```text
src/WebhookInbox.Api/Properties/launchSettings.json
```

## Health Check

```http
GET /health
```

A successful response returns HTTP `200 OK`.

Example response:

```json
{
  "status": "ok"
}
```

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/health` | Check API availability |
| POST | `/api/events` | Receive and store a webhook event |
| GET | `/api/events` | Return all stored webhook events |
| GET | `/api/events/{id}` | Return a webhook event by ID |
| DELETE | `/api/events/{id}` | Delete a webhook event by ID |

## Create a Webhook Event

### Request

```http
POST /api/events
Content-Type: application/json
```

Example body:

```json
{
  "source": "github",
  "eventType": "push",
  "payload": "{\"ref\":\"refs/heads/main\"}"
}
```

The client provides:

- `source`
- `eventType`
- `payload`

The server generates:

- `id`
- `receivedAt`

### Successful response

A successfully created event returns:

```text
201 Created
```

Example response:

```json
{
  "id": "450da462-8fde-4d72-b88b-24688ca97727",
  "receivedAt": "2026-10-01T20:30:00+00:00",
  "source": "github",
  "eventType": "push",
  "payload": "{\"ref\":\"refs/heads/main\"}"
}
```

The response also contains a `Location` header
