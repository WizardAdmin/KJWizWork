@echo off
title KB4074588 ������Ʈ ���� ����
bcdedit /enum {bootmgr} > nul || goto _admin

wusa /uninstall /kb:4074588 /quiet /norestart

pause


:_admin
color 4f
cls
echo.
echo.
echo.
echo          ������ �������� ������� �ʾҽ��ϴ�.
echo.
echo          ��Ŭ�� �ؼ� ������ �������� �������ּ���.
echo.
echo.
echo.
pause
exit