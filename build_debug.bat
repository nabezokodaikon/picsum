@SET PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin";%PATH%
@SET OUTPUT_PATH=%CD%\bin_debug

DEL /Q "%OUTPUT_PATH%\*.*"
RD /S /Q "%OUTPUT_PATH%\runtimes"

dotnet restore src\PicSum.sln

MSBuild src\PicSum.sln /t:Rebuild ^
  /p:Configuration=Debug ^
  /p:OutputPath="%OUTPUT_PATH%" ^
  /p:Platform="Any CPU"
