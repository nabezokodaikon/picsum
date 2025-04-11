@SET OUTPUT_PATH=%CD%\bin_develop

DEL /Q "%OUTPUT_PATH%\*.*"
RD /S /Q "%OUTPUT_PATH%\runtimes"

dotnet clean src\PicSum.Main\PicSum.Main.csproj ^
  -c Develop ^
  -p:Platform="x64"

dotnet build src\PicSum.Main\PicSum.Main.csproj ^
  -c Develop ^
  -o "%OUTPUT_PATH%" ^
  -p:Platform="x64"
