"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe" /m:8 /p:Configuration=Release "Passenger.sln"

REM For building symbols
"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe" /m:8 /p:Configuration=Debug "Passenger.sln"