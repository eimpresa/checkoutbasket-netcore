default: dist

clean:
	@echo "Begin cleanup"
	rm -rf build/
	rm -rf dist/

dist: clean
	mkdir build
	mkdir dist
	dotnet restore CheckoutBasket.sln
	dotnet build --configuration Release --no-restore
	dotnet publish --configuration Release --no-build --no-restore --output build/ CheckoutBasket/CheckoutBasket.csproj

test:
	dotnet test --configuration Release --no-build --no-restore CheckoutBasket.UnitTests/CheckoutBasket.UnitTests.csproj