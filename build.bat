@echo off
SET mypath=%~dp0
dotnet restore CheckoutBasket.sln
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build --no-restore CheckoutBasket.UnitTests\CheckoutBasket.UnitTests.csproj
dotnet publish --configuration Release --no-build --no-restore --output %mypath:~0,-1%\build CheckoutBasket\CheckoutBasket.csproj
echo Build OK