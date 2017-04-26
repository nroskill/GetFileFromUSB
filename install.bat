@echo off
%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\installutil.exe %~dp0GetFile.exe
net start GetFile
pause