SETLOCAL

SET LAYERS=layers.dll
SET THIS=%~dp0

SET LIBRARY=%LIBRARY%\Layers\
SET CMODULE="%CCOMMON%\"
SET VMODULE="%VCOMMON%\"

ECHO. >> %LOG%
ECHO  ^>^>^> LIBRARY/LAYERS ^>^>^>

ECHO. >> %LOG%
ECHO  *  %THIS% --^> %LIBRARY% >> %LOG%
ECHO  *  %THIS% --^> %LIBRARY%
ECHO. >> %LOG%

%CP% "%THIS%%LAYERS%" "%LIBRARY%" 1>> %LOG% 2>>&1

ECHO. >> %REGCMD%
ECHO REGSVR32 /S "%LIBRARY%%LAYERS%" >> %REGCMD%

ECHO. >> %LOG%
ECHO  *  %THIS% --^> %CMODULE% >> %LOG%
ECHO  *  %THIS% --^> %CMODULE%
ECHO. >> %LOG%

%CP% "%THIS%Tenaris.*.dll" %CMODULE% 1>> %LOG% 2>>&1

ECHO. >> %LOG%
ECHO  *  %THIS% --^> %VMODULE% >> %LOG%
ECHO  *  %THIS% --^> %VMODULE%
ECHO. >> %LOG%

%CP% "%THIS%Tenaris.*.dll" %VMODULE% 1>> %LOG% 2>>&1

ECHO  ^<^<^< DONE ^<^<^<

ENDLOCAL
