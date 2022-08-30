@echo off
setlocal

set prefix=ID3-AutoGen
set rar="%ProgramW6432%\WinRAR\winrar.exe"

echo Enter new version number in format X.Y.Z:
set /p version=""

echo Building...
echo ...for win-x64
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false -o dist\win-x64> nul
echo ...for linux-x64
dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained false -o dist\linux-x64 > nul
echo ...for osx-x64
dotnet publish -c Release -r osx-x64 -p:PublishSingleFile=true --self-contained false -o dist\osx-x64 > nul

cd dist

echo Compressing...
%rar% a -ep %prefix%-%version%-win.zip win-x64\*.*
%rar% a -ep %prefix%-%version%-linux.zip linux-x64\*.*
%rar% a -ep %prefix%-%version%-osx.zip osx-x64\*.*

echo Signing...
gpg -a --detach-sign %prefix%-%version%-win.zip
gpg -a --detach-sign %prefix%-%version%-linux.zip
gpg -a --detach-sign %prefix%-%version%-osx.zip

echo done.