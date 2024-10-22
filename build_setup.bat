@SET PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin";%PATH%
@SET OUTPUT_PATH=%CD%\bin_setup

rem dotnet restore src\PicSum.sln

MSBuild src\Setup.sln /t:Rebuild ^
  /p:OutputPath="%OUTPUT_PATH%" ^
  /p:Configuration=Release ^
  /p:Platform="Any CPU"
