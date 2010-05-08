@echo off

set currentpath=%~dp0

set dt2=%date:~-10%
set dt=%dt2:/=%

rem for /f %%i in ('subwcrev ') do
rem 	set ver=%%i
set ver=x.xx.x.xx

set status=snapshot

set outfilename=Tumblott_v%ver%_%status%_%dt%.CAB
echo %outfilename%

cd "C:\Program Files\Windows CE Tools\wce420\POCKET PC 2003\Tools"
Cabwiz.exe "%currentpath%Tumblott.inf"

cd %currentpath%
ren Tumblott.CAB %outfilename%

