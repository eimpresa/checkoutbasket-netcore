Checkout Basket
=========
A simple shopping cart web api and client implementation based on .NET Core 2.0

Requisites
------------------------
- You must have the `.NET Core 2.0 SDK` installed
- dotnet.exe must be in your PATH

Web API
=========

Build
------------------------
To build the solution, simply run the build script included in the root folder of the solution. This will generate production-ready binaries in the `build` folder.
    
    build.bat

Run
------------------------
    cd build
    dotnet CheckoutBasket.dll
    
Regression
------------------------

A basic regression tests suite is included in the solution. Once your CheckoutBasket Web API is up and running, you can execute regression tests by using the following command.

    dotnet test --configuration Release --no-build --no-restore "CheckoutBasket.Client.RegressionTests"
    
Client
=========

Configure the client
------------------------    

    var client = new CheckoutBasketClient("YOUR_API_URL", new NetworkCredential("YOUR_API_KEY", "YOUR_API_SECRET");

Create a new order
------------------------
First you need to create an order to get an order id.

    var res = await client.CreateOrder();
    if(!res.Ok)
    {
        // error handling
    }
    
    var orderId = res.Data.Id;
    
    
Add new items
------------------------    

    var res = await client.AddItem(orderId, new Guid("8b227969-a42a-4055-b22d-3a21396375cf"), 100);
    if(!res.Ok)
    {
        // error handling
    }

Delete items from an order
------------------------    

    var res = await client.DeleteItem(orderId, new Guid("8b227969-a42a-4055-b22d-3a21396375cf"));
    if(!res.Ok)
    {
        // error handling
    }
    
Update item quantity
------------------------    

    var res = await client.UpdateItem(orderId, new Guid("8b227969-a42a-4055-b22d-3a21396375cf"), 99);    
    if(!res.Ok)
    {
        // error handling
    }
    
Clear an order
------------------------    

    var res = await client.ClearOrder(orderId);
    if(!res.Ok)
    {
        // error handling
    }    
