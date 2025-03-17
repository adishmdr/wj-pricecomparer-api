# StarWars Movie Comparer - Backend

Welcome to the backend of the StarWars Movie Comparer! This ASP.NET Core Web API fetches and compares movie data from CinemaWorld and FilmWorld, integrating with the frontend via a Star Wars-themed design. It’s built to be robust, secure, and efficient.

## Features
- **Movie Data Fetching**: Retrieves movie lists and details from CinemaWorld and FilmWorld APIs.
- **Price Comparison**: Compares prices for specific movie IDs, allowing empty IDs for flexibility.
- **Token Authentication**: Uses the `x-access-token` header (e.g., `3434332sdfafas`) from the frontend for Webjet API calls.
- **Caching Strategy**: Caches data only when both CinemaWorld and FilmWorld datasets are non-empty, with a 5-minute duration.
- **Retry Mechanism**: Uses Polly for retrying failed Webjet API calls.
- **Logging**: Tracks rea
---

### Backend `README.md`

```markdown
# StarWars Movie Comparer - Backend

Welcome to the backend of the StarWars Movie Comparer! This ASP.NET Core Web API fetches and compares movie data from CinemaWorld and FilmWorld, integrating with the frontend via a Star Wars-themed design. It’s built to be robust, secure, and efficient.

## Features
- **Movie Data Fetching**: Retrieves movie lists and details from CinemaWorld and FilmWorld APIs.
- **Price Comparison**: Compares prices for specific movie IDs, allowing empty IDs for flexibility.
- **Token Authentication**: Uses the `x-access-token` header (e.g., `234324sdfsdf`) from the frontend for Webjet API calls.
- **Caching Strategy**: Caches data only when both CinemaWorld and FilmWorld datasets are non-empty, with a 5-minute duration.
- **Retry Mechanism**: Uses Polly for retrying failed Webjet API calls.
- **Logging**: Tracks requests, responses, and errors for debugging.

## Tech Stack
- **ASP.NET Core**: For building the RESTful API.
- **C#**: For backend logic and data handling.
- **Polly**: For retry policies.
- **MemoryCache**: For temporary data storage.
- **HttpClientFactory**: For managing Webjet API requests.

## Installation Process

### Prerequisites
- **.NET SDK**: Version 6 or higher (download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download))
- **Visual Studio**: Or your preferred C# IDE (e.g., Visual Studio Code with C# extension)
- **Git**: For cloning the repository (download from [git-scm.com](https://git-scm.com/))



## Tech Stack
- **ASP.NET Core**: For building the RESTful API.
- **C#**: For backend logic and data handling.
- **Polly**: For retry policies.
- **MemoryCache**: For temporary data storage.
- **HttpClientFactory**: For managing Webjet API requests.
