@SET OUTPUT_PATH=%CD%\bin_publish

DEL /Q "%OUTPUT_PATH%\*.*"

dotnet restore src\PicSum.Main\PicSum.Main.csproj

dotnet publish src\PicSum.Main\PicSum.Main.csproj ^
  -c Release ^
  -o "%OUTPUT_PATH%" ^
  -p:Platform="x64"
