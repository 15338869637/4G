echo.
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe %~dp0Fujica.com.cn.MonitorService.exe
Net Start Fujica.com.cn.MonitorService
sc config Fujica.com.cn.MonitorService start= auto

pause