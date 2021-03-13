![Tests](https://github.com/coderookie1994/Microstack/actions/workflows/tests.yml/badge.svg)
# Microstack
### Seamlessly run .net core apps using this CLI to avoid opening multiple code editors or instances of visual studio.

This CLI tool was primarily built to support non docker based workflows, where manually multiple services need to be spanwed up to work with the application stack.
The idea is to ease day to day development by being concerned with only what is required, i.e to run the bare minimum. The CLI starts all the services specified in the background,
so the developer is fully focused on the service that he/she is developing.

If you have microservices like this where <br/>
  a) A ---> B ---> C<br/>
  b) A ---> B ---> C<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|----> D<br/>
     ---> dependsOn<br/>
If you work on C, to test it end to end you would have to run A and B. The json configuration allows you to set configuration urls to override the default config, by setting
the overridden environment variables directly into the process's environment for instance :-
```json
"profile2": [
    {
      "StartupProjectPath": "C:\\github.com\\ServiceB",
      "ProjectName": "ServiceB",
      "NextProjectName": "ServiceC",
      "GitProjectRootPath": "C:\\github.com\\ServiceB",
      "PullLatest": false,
      "Port": 5001,
      "Verbose": false,
      "ConfigOverrides": {
        "ServiceC": "https://localhost:6001",
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  ]
```
The configOverrides object contains the "ServiceC" value which has to be overridden.

![Microstack CLI](resources/microstack.bmp?raw=true "Microstack")

Use the "new" command to generate config files with the [options] switch.
The JSON configuration can be used either by providing the [-c] switch for path, or set the "MSTKC_JSON" environment variable to point to the configuration file
![New subcommand](resources/new.bmp?raw=true "New")

To run the services, use the "run" command with [options]
![Run subcommand](resources/run.bmp?raw=true "Run")

To stop microstack, press CTRL + C
