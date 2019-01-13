mkfile_path := $(abspath $(lastword $(MAKEFILE_LIST)))
mkfile_dir := $(dir $(mkfile_path))

default: dist

clean:
	@echo "Begin cleanup"
	rm -rf build/
	rm -rf dist/

dist: clean
	mkdir build
	mkdir dist
	dotnet restore ./src/CheckoutBasket.sln
	dotnet build --configuration Release --no-restore ./src/CheckoutBasket.sln
	dotnet publish --configuration Release --no-build --no-restore --output $(mkfile_dir)/build ./src/CheckoutBasket/CheckoutBasket.csproj
	cp checkoutbasket ./build/

test:
	dotnet test --configuration Release --no-build --no-restore ./src/CheckoutBasket.UnitTests/CheckoutBasket.UnitTests.csproj

regression:
	dotnet test --configuration Release --no-build --no-restore ./src/CheckoutBasket.Client.RegressionTests/CheckoutBasket.Client.RegressionTests.csproj

run:
	./build/checkoutbasket