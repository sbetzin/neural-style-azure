@echo off

REM https://github.com/dwmkerr/sharpshell/blob/master/docs/installing/installing.md
REM https://github.com/dwmkerr/sharpshell/blob/master/docs/srm/srm.md
C:\Data\Development\Private\neural-style-azure\deploy\ExplorerExtension\ServerRegistrationManager.exe config LoggingMode File
C:\Data\Development\Private\neural-style-azure\deploy\ExplorerExtension\ServerRegistrationManager.exe config LogPath C:\Data\Development\Private\neural-style-azure\deploy\
C:\Data\Development\Private\neural-style-azure\deploy\ExplorerExtension\ServerRegistrationManager.exe install C:\Data\Development\Private\neural-style-azure\deploy\ExplorerExtension\NeuralStyle.ExplorerExtension.dll -codebase
