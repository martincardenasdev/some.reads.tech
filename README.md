### Configure appsettings.json
```
{
  "GeminiApiKey": "Gemini API Key here",
  "ConnectionStrings": {
    "DefaultConnection": "Postgres database connection string here",
    "Redis": "Redis connection string here"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Secret": "JWT Secret here",
    "Issuer": "some.reads.tech",
    "Audience": "some.reads.tech"
  }
}
```
