@SET PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin";%PATH%
@SET OUTPUT_PATH=%CD%\bin_debug

MSBuild src\PicSum.sln /t:Rebuild ^
  /p:OutputPath="%OUTPUT_PATH%" ^
  /p:Configuration=Debug ^
  /p:Platform="Any CPU"

PAUSE
