@SET PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin";%PATH%
@SET OUTPUT_PATH="%CD%\bin_release"

RD /s /q %OUTPUT_PATH%

MSBuild src\PicSum.sln /t:Rebuild ^
  /p:OutputPath="%CD%\bin_release" ^
  /p:Configuration=%OUTPUT_PATH% ^
  /p:Platform="Any CPU" ^
  /p:GenerateDependencyFile=false

DEL "%OUTPUT_PATH%\*.pdb"
DEL "%OUTPUT_PATH%\*.xml"

PAUSE
