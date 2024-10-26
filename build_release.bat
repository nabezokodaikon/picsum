@SET PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin";%PATH%
@SET OUTPUT_PATH=%CD%\bin_release

RD /S /Q "%OUTPUT_PATH%"

dotnet restore src\PicSum.sln

MSBuild src\PicSum.sln /t:Rebuild ^
  /p:Configuration=Release ^
  /p:OutputPath="%OUTPUT_PATH%" ^
  /p:Platform="x64"
