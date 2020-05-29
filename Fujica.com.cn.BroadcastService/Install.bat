@echo off 
set filename=E:\SourceForSvn\Fujica.com.cn.4GCamera\Fujica.com.cn.BroadcastService\bin\Debug\Fujica.com.cn.BroadcastService.exe
set servicename=Fujica.4GCamera.BroadcastService
pause
echo ============================操作日志==================================== >InstallService.log  
if exist "%SystemRoot%\Microsoft.NET\Framework\v4.0.30319" goto netOld 
:DispError 
echo 您的机器上没有安装 .net Framework 4.0,安装即将终止 
echo 您的机器上没有安装 .net Framework 4.0,安装即将终止 >>InstallService.log  
goto LastEnd 
:netOld 
cd %SystemRoot%\Microsoft.NET\Framework\v4.0.30319 
echo 您的机器上安装了相应的.net Framework 4.0,可以安装本服务.
echo 您的机器上安装了相应的.net Framework 4.0,可以安装本服务 >>InstallService.log  
echo off 
echo 清理原有服务项. . .
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil /U %filename% >> InstallService.log
echo 清理完毕
echo.
echo *********************
echo 安装服务
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil %filename% >> InstallService.log
echo 启动服务
net start %servicename% >> InstallService.log
echo *********************
echo ======================================================================= >>InstallService.log 
type InstallService.log
echo.
echo 操作结束，可以查看日志文件InstallService.log 中具体的操作结果。
:LastEnd 
pause
rem exit