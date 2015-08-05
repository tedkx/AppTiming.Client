@echo off

set /p APIKEY=<apikey.txt

nuget pack ..\src\AppTiming.Client\AppTiming.Client.csproj > temp.txt

:: Get the last line of PACK output file
set LASTLINE=
for /f "delims==" %%a in (temp.txt) do (
	set LASTLINE=%%a
)

:: TODO: Check pack output for errors

:: Get created filename from pack output
SET PATTERN=Successfully created package '%CD%\
CALL SET FILENAME=%%LASTLINE:%PATTERN%=%%
SET PATTERN='.
CALL SET FILENAME=%%FILENAME:%PATTERN%=%%

:: Delete pack output file
del /F /Q temp.txt

:: Push package to nuget.org
nuget push %FILENAME% -ApiKey %APIKEY%