# Student Register

Student Register is a .NET 10 teaching solution containing:

- `StudentRegister.Api`: an ASP.NET Core Web API backed by SQL Server;
- `StudentRegister.Web`: an ASP.NET Core MVC client;
- `StudentRegister.Api.Tests`: controller and authorization tests.

The API demonstrates cookie authentication and role-based authorization with `Viewer` and `Admin` roles, following the same pattern used in the Employee Directory project.

> The demonstration usernames and passwords are hard-coded for learning purposes. Never use this approach for production accounts.

## Prerequisites

Install the following before opening the solution:

1. [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
2. Visual Studio 2026 with the **ASP.NET and web development** workload, or Visual Studio Code with C# Dev Kit
3. SQL Server Express, SQL Server Developer, or LocalDB
4. Git

Confirm the SDK is available:

```powershell
dotnet --version
```

The output must begin with `10.`.

## 1. Clone and open the solution

```powershell
git clone <repository-url>
cd StudentRegister.Api
dotnet restore StudentRegister.Api.slnx
```

Open `StudentRegister.Api.slnx` in Visual Studio, or open the repository folder in Visual Studio Code.

## 2. Configure SQL Server

Update `StudentRegister.Api/appsettings.json` so `DefaultConnection` points to your SQL Server instance.

SQL Server Express example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=StudentsDB;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

LocalDB example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=StudentsDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

The API applies its Entity Framework Core migrations when it starts. To apply them manually instead, install the EF tool and run:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef database update --project StudentRegister.Api\StudentRegister.Api.csproj
```

If `dotnet-ef` is already installed, use `dotnet tool update --global dotnet-ef`.

## 3. Trust the development HTTPS certificate

```powershell
dotnet dev-certs https --trust
```

## 4. Run the application

Open a PowerShell terminal in the repository root and start the API:

```powershell
dotnet run --project StudentRegister.Api\StudentRegister.Api.csproj --launch-profile https
```

The API listens on `https://localhost:7251` (and `http://localhost:5078`) and opens Swagger UI automatically.

To also run the MVC web client, open a second terminal:

```powershell
dotnet run --project StudentRegister.Web\StudentRegister.Web.csproj --launch-profile https
```

In Visual Studio, configure multiple startup projects and set both `StudentRegister.Api` and `StudentRegister.Web` to **Start**.

## Demonstration users

| Username | Password | Role | Permissions |
| --- | --- | --- | --- |
| `viewer` | `viewer123` | Viewer | List, search, and view students |
| `admin` | `admin123` | Admin | List, search, view, add, update, and delete students |

An unauthenticated API request receives `401 Unauthorized`. A signed-in Viewer who calls an Admin-only endpoint receives `403 Forbidden`.

## Role-based access requirements

Role-based access works only when authentication and authorization are configured together. Authentication identifies the user, while authorization checks whether that user has the required role.

The following pieces are required in this solution.

### 1. Register cookie authentication and authorization

The API must register cookie authentication as its default authentication scheme and add authorization services:

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "StudentRegister.Auth";
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();
```

Cookie authentication normally redirects users to an HTML login page. Because this project exposes an API, the redirect events return `401 Unauthorized` and `403 Forbidden` instead.

### 2. Add middleware in the correct order

`StudentRegister.Api/Program.cs` must call authentication before authorization and before mapping controllers:

```csharp
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
```

`UseAuthentication` reads the encrypted cookie and creates `HttpContext.User`. `UseAuthorization` then compares that user's claims with the endpoint requirements. Reversing or omitting these calls prevents role checks from working correctly.

### 3. Create a role claim during login

A successful login must create both a name claim and a role claim:

```csharp
var claims = new[]
{
    new Claim(ClaimTypes.Name, username),
    new Claim(ClaimTypes.Role, role)
};
```

The role value must exactly match one of the constants in `StudentRegister.Api/Authorization/AppRoles.cs`. Role names are case-sensitive in normal role checks. A cookie without a `ClaimTypes.Role` claim can authenticate the user but cannot authorize role-protected endpoints.

### 4. Issue and return the authentication cookie

After creating the claims, the login endpoint must call `HttpContext.SignInAsync`. ASP.NET Core encrypts the identity and sends it in the `StudentRegister.Auth` cookie.

Clients must retain that cookie and send it with later API requests:

```http
Cookie: StudentRegister.Auth=<encrypted-value>
```

### 5. Protect controllers and actions

The student controller allows both roles to read:

```csharp
[Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Viewer}")]
public class StudentController : ControllerBase
```

Write actions add a stricter Admin requirement:

```csharp
[Authorize(Roles = AppRoles.Admin)]
```

The resulting access rules are:

| Student operation | Anonymous | Viewer | Admin |
| --- | --- | --- | --- |
| List, search, or view students | `401` | Allowed | Allowed |
| Add, update, or delete students | `401` | `403` | Allowed |

UI checks are only for usability. Hiding Admin buttons from a Viewer does not secure the API; the API controller must always enforce the role requirement.

### 6. Verify every access path

Role-based access should be tested through the complete HTTP middleware pipeline, not only by calling controller methods directly. At minimum, verify:

1. an anonymous read returns `401`;
2. a Viewer read succeeds;
3. a Viewer write returns `403` without calling the student service;
4. an Admin write reaches the student service;
5. invalid credentials return `401`;
6. logout removes the authenticated session.

These cases are covered by `StudentRegister.Api.Tests/AuthorizationTests.cs`.

## How cookie authentication works

1. The user submits credentials to `POST /api/auth/login`.
2. The API validates the demonstration account and creates name and role claims.
3. ASP.NET Core returns an encrypted, HTTP-only `StudentRegister.Auth` cookie.
4. The cookie is sent with later API requests.
5. The student controller allows both roles to read data and requires `Admin` for changes.
6. `POST /api/auth/logout` invalidates the authentication session.

API authentication endpoints:

| Method | Route | Purpose |
| --- | --- | --- |
| `POST` | `/api/auth/login` | Validate credentials and issue a cookie |
| `GET` | `/api/auth/me` | Return the current username and role |
| `POST` | `/api/auth/logout` | Remove the authentication cookie |

API student endpoints:

| Method | Route | Role | Description |
| --- | --- | --- | --- |
| `GET` | `/api/student` | Viewer, Admin | List all students |
| `GET` | `/api/student/search?search={term}` | Viewer, Admin | Search by student number, first name, or last name |
| `GET` | `/api/student/{id}` | Viewer, Admin | Get a single student |
| `POST` | `/api/student` | Admin | Create a student |
| `PUT` | `/api/student/{id}` | Admin | Update a student |
| `DELETE` | `/api/student/{id}` | Admin | Delete a student |

## Test the API manually

Open `StudentRegister.Api/StudentRegister.Api.http` in Visual Studio or Visual Studio Code with the REST Client extension. Run requests in this order:

1. Viewer login
2. Student list - expect `200 OK`
3. Delete a student as Viewer - expect `403 Forbidden`
4. Admin login
5. Add a student - expect `200 OK`
6. `GET /api/auth/me` - expect the Admin username and role
7. Logout

## Automated tests and continuous integration

`StudentRegister.Api.Tests` contains two test classes:

- `StudentControllerTests` - unit tests that call controller actions directly against a mocked `IStudentService`.
- `AuthorizationTests` - integration tests that boot the full ASP.NET Core pipeline with `WebApplicationFactory<Program>` and verify the `401`/`403`/`200`/`202` behavior described above.

Run all tests locally:

```powershell
dotnet test StudentRegister.Api.slnx --configuration Release
```

[`.github/workflows/unit-tests.yml`](.github/workflows/unit-tests.yml) runs the same restore, build, and test steps automatically on every push to `main`/`master` and on every pull request, using the `.NET 10` SDK on `windows-latest`.
