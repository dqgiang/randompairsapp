# Random Pairs App

## Prerequisites

- .NET Core 2.x with CLI tool installed
- A web browser
- Port 5000 at localhost is available

## How to run

### Backend service

- Open the root folder (`RandomPairsApp`) in a terminal (or command prompt)
- Run `dotnet build`
- Run `dotnet run`

The above run commands should succeed.

### Web app

- Open the `ìndex.html` file under `WebApp` folder in a web browser
- The web page should display the latest random pairs automatically
- Clicking on the `Get Sum` or `Get Median` buttons should show the corresponding values

> Note:
> - If there are more than 10 numbers available in the list to calculate sum, the sum function returns the total of the 10 oldest numbers.
> - If there are less than 10 eligible numbers (not counting the ones already used), however, the sum function will return the total of those numbers only.
> - If there are 0 number to sum, the function return error code 500.

## Contact

[giangq.dam@gmail.com](mailto:giangq.dam@gmail.com)