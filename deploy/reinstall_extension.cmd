@echo off
xcopy C:\Data\Development\Private\neural-style-azure\src\NeuralStyle.ExplorerExtension\bin\Debug\*.exe C:\Data\Development\Private\neural-style-azure\deploy\ExplorerExtension\ /Y
xcopy C:\Data\Development\Private\neural-style-azure\src\NeuralStyle.ExplorerExtension\bin\Debug\*.dll C:\Data\Development\Private\neural-style-azure\deploy\ExplorerExtension\ /Y
xcopy C:\Data\Development\Private\neural-style-azure\src\NeuralStyle.ExplorerExtension\bin\Debug\*.config C:\Data\Development\Private\neural-style-azure\deploy\ExplorerExtension\ /Y

C:\Data\Development\Private\neural-style-azure\deploy\ExplorerExtension\ServerRegistrationManager.exe uninstall C:\Data\Development\Private\neural-style-azure\deploy\ExplorerExtension\NeuralStyle.ExplorerExtension.dll

C:\Data\Development\Private\neural-style-azure\deploy\ExplorerExtension\ServerRegistrationManager.exe install C:\Data\Development\Private\neural-style-azure\deploy\ExplorerExtension\NeuralStyle.ExplorerExtension.dll -codebase


