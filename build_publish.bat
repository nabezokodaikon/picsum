@SET OUTPUT_PATH=%CD%\bin_publish

DEL /Q "%OUTPUT_PATH%\*.*"

dotnet clean src\PicSum.Main\PicSum.Main.csproj ^
  -c Release ^
  -p:Platform="x64"

dotnet publish src\PicSum.Main\PicSum.Main.csproj ^
  -c Release ^
  -o "%OUTPUT_PATH%" ^
  -p:Platform="x64"
