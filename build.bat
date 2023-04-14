@SET PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin";%PATH%
MSBuild src\PicSum.sln /p:OutputPath="%CD%\bin" /t:Clean;Rebuild /p:Configuration=Release;Platform="Any CPU"

del "%CD%\bin\*.pdb"
del "%CD%\bin\*.xml"

pause