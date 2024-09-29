# Environment setup
During development to provide environment variables to the solution create a file named env.json at the root of the project folder. for example:

**Do NOT include this file in source control**
```json
{
    "SELENIUM_BROWSER" : "chrome",
    "SELENIUM_HEADLESS" : "false",
    "SELENIUM_WAIT" : 3,
    "D365_URL" : "",
    "D365_USERNAME" :"",
    "D365_PASSWORD" : "",
    "D365_CLIENT_ID" : "",
    "D365_CLIENT_SECRET_KEY": "",
    "D365_TENANT_ID": ""
}
```
## CI/CD Execution
```sh
dotnet test --test-adapter-path:. --logger:junit
```