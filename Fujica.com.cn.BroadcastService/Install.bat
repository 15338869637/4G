@echo off 
set filename=E:\SourceForSvn\Fujica.com.cn.4GCamera\Fujica.com.cn.BroadcastService\bin\Debug\Fujica.com.cn.BroadcastService.exe
set servicename=Fujica.4GCamera.BroadcastService
pause
echo ============================������־==================================== >InstallService.log  
if exist "%SystemRoot%\Microsoft.NET\Framework\v4.0.30319" goto netOld 
:DispError 
echo ���Ļ�����û�а�װ .net Framework 4.0,��װ������ֹ 
echo ���Ļ�����û�а�װ .net Framework 4.0,��װ������ֹ >>InstallService.log  
goto LastEnd 
:netOld 
cd %SystemRoot%\Microsoft.NET\Framework\v4.0.30319 
echo ���Ļ����ϰ�װ����Ӧ��.net Framework 4.0,���԰�װ������.
echo ���Ļ����ϰ�װ����Ӧ��.net Framework 4.0,���԰�װ������ >>InstallService.log  
echo off 
echo ����ԭ�з�����. . .
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil /U %filename% >> InstallService.log
echo �������
echo.
echo *********************
echo ��װ����
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil %filename% >> InstallService.log
echo ��������
net start %servicename% >> InstallService.log
echo *********************
echo ======================================================================= >>InstallService.log 
type InstallService.log
echo.
echo �������������Բ鿴��־�ļ�InstallService.log �о���Ĳ��������
:LastEnd 
pause
rem exit