# binance-websockets-task

## WEB API

1. Run docker container (Recommended)

Clone ther repository, navigate to the root folder and execute the following bash command **sh run_web_api.sh**. Once the command finishes executing, the Web Api will be available at http://localhost:5000. If port 5000 is taken by another application, consider stopping your other application or changing the exposed docker port in the run_web_api.sh file at line 8  "...-p 5000:80...".

2. Run the WebApi project from IDE

Using your IDE of choice open the BinanceWebSocketTask.sln file. 

- In VS, set the startup project to BinanceWebSocketTask.WebApi and run the application.

- In Rider, either choose the BinanceWebSocketTask.WebApi:IIS express or BinanceWebSocketTask run configuration and run the application

Once the application has started, depending on the executing profile, it will be available at at one of the following addresses: http://localhost:5002, https://localhost:5003, http://localhost:63279, http://localhost:44349

## Console App

1. Run the WebApi project from IDE (Recommended)

Using your IDE of choice open the BinanceWebSocketTask.sln file. 

- In VS, set the startup project to BinanceWebSocketTask.ConsoleApp and run the application.

- In Rider, either choose the BinanceWebSocketTask.ConsoleApp run configuration and run the application

Once the application has started you will be able to interact with the console. Follow the instructions on screen to interact with the application.

2. Run docker container (Not recommended due to https://github.com/dotnet/aspnetcore/issues/27675)

Clone ther repository, navigate to the root folder and execute the following bash command **sh run_console_app.sh**. Once the command finishes executing, the Console App will start in non-detached mode. Follow the instructions on screen to interact with the application.


## Additional information

By default the application will always drop the database on startup. This is intended to ensure clean slate on startup and no dirty noise in terms of data. To disable this behaviour simply go to binance-websockets-task\BinanceWebSocketTask.Data\Data\BinanceWebSocketTaskDbContext.cs and comment line 22 `await this.Database.EnsureDeletedAsync();`
