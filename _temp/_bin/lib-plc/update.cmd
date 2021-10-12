SETLOCAL

SET PLC=plc.dll
SET THIS=%~dp0

SET LIBRARY=%LIBRARY%\Layers\
SET CMODULE="%CCOMMON%\"
SET VMODULE="%VCOMMON%\"

ECHO. >> %LOG%
ECHO  ^>^>^> LIBRARY/PLC ^>^>^>

ECHO. >> %LOG%
ECHO  *  %THIS% --^> %LIBRARY% >> %LOG%
ECHO  *  %THIS% --^> %LIBRARY%
ECHO. >> %LOG%

%CP% "%THIS%%PLC%" "%LIBRARY%" 1>> %LOG% 2>>&1

ECHO. >> %REGCMD%
ECHO REGSVR32 /S "%LIBRARY%%PLC%" >> %REGCMD%

ECHO. >> %LOG%
ECHO  *  %THIS% --^> %CMODULE% >> %LOG%
ECHO  *  %THIS% --^> %CMODULE%
ECHO. >> %LOG%

%CP% "%THIS%Tenaris.*.dll" %CMODULE% 1>> %LOG% 2>>&1

ECHO  ^<^<^< DONE ^<^<^<

ENDLOCAL
