@SET PICSUM_FILES=%USERPROFILE%\AppData\Local\Packages\NabezokoDaikon.49446294FC8C5_3qnydrnvfr8aa\LocalState\picsum.files

RMDIR /S /Q %PICSUM_FILES%
MKDIR %PICSUM_FILES%

MKLINK /D %PICSUM_FILES%\db %CD%\bin_publish\db
MKLINK /D %PICSUM_FILES%\config %CD%\bin_publish\config
MKLINK /D %PICSUM_FILES%\log %CD%\bin_publish\log
