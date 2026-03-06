# checkstastusapinew — Production Deployment Notes

Quick guide to build, run, and deploy the API in production.

Build and run locally (Release):

```powershell
dotnet publish -c Release -o ./publish
dotnet ./publish/checkstastusapinew.dll
```

Docker (build and run):

```powershell
# build
docker build -t checkstastusapinew:latest .

# run (maps container port 80 to host 8080)
docker run -e ASPNETCORE_ENVIRONMENT=Production -p 8080:80 checkstastusapinew:latest
```

GitHub Actions CI:

- The workflow at `.github/workflows/ci.yml` restores, builds, and publishes the project.
- If you set `DOCKERHUB_USERNAME` and `DOCKERHUB_TOKEN` in repository secrets, the workflow will also build and push an image.

Environment variables and secrets:

- `ASPNETCORE_ENVIRONMENT=Production` — run the app in Production mode.
- `ASPNETCORE_URLS` — the Dockerfile sets `http://+:80` inside container. Override if needed.
- `DefaultConnection` — set your production connection string via secret management (not in source files).
- For TLS on bare Kestrel, configure certificates outside source control and provide via OS certificate store or environment variables and configure Kestrel accordingly.

Health checks and probes:

- Liveness: `GET /health`
- Readiness: `GET /ready`

Swagger in Production (safe option)

- You can enable Swagger in Production by setting the environment variable `ENABLE_SWAGGER=true`.
- For safety, the app supports protecting Swagger with an API key. Set `SWAGGER_API_KEY` to a secret value and then send the header `X-SWAGGER-API-KEY: <value>` with requests to `/swagger`.
- Example `web.config` snippet to enable Swagger and set the key (for IIS):

```xml
<aspNetCore processPath="dotnet" arguments=".\checkstastusapinew.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess">
	<environmentVariables>
		<environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
		<environmentVariable name="ENABLE_SWAGGER" value="true" />
		<environmentVariable name="SWAGGER_API_KEY" value="<YOUR_SECRET_KEY>" />
	</environmentVariables>
</aspNetCore>
```

Note: Enabling Swagger in Production should be temporary and guarded. Use the API-key protection or restrict access via network/firewall rules. Remove `ENABLE_SWAGGER` or clear `SWAGGER_API_KEY` after debugging.

Security and reverse proxies:

- The app is configured to respect common forwarded headers (X-Forwarded-For, X-Forwarded-Proto).
- Basic security response headers and HSTS are enabled for non-development environments.

Next steps you may want me to do:

- Add automated tests and a test job in CI.
- Configure a production-ready certificate setup (Let's Encrypt or managed TLS behind a load balancer).
- Add monitoring (Application Insights) and structured logging (e.g., Serilog).
