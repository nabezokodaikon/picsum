@SET OUTPUT_PATH=%CD%\bin_debug

DEL /Q "%OUTPUT_PATH%\*.*"
RD /S /Q "%OUTPUT_PATH%\runtimes"

dotnet restore src\PicSum.Main\PicSum.Main.csproj 

dotnet clean src\PicSum.Main\PicSum.Main.csproj ^
  -c Debug ^
  -p:Platform="x64"

dotnet build src\PicSum.Main\PicSum.Main.csproj ^
  -c Debug ^
  -o "%OUTPUT_PATH%" ^
  -p:Platform="x64"
