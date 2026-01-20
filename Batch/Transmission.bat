@echo off

chcp 65001 > nul

set CUR=%~dp0
set EXE_NAME=Batch.exe

"%CUR%%EXE_NAME%" %1 %2 %3

if %ERRORLEVEL% equ 0 (
  echo 正常終了
) else (
  echo 異常終了
)

exit /b %ERRORLEVEL%

pause