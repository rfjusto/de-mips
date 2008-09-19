rem ALWAYS run me before commiting to the source tree.

echo clean debug folder
cd DeAssembly
cd bin
cd Debug
del *.exe
del *.manifest
del *.pdb

echo clean release folder
cd..
cd release
del *.exe
del *.manifest
del *.pdb

echo clean object files
cd..
cd..
cd obj
cd Debug
del *.txt
del *.cache
del *.exe
del *.resources
del *.pdb
cd TempPE
del Properties.Resources.Designer.cs.dll

cd..\..
cd Release
del *.txt
del *.cache
del *.exe
del *.resources
del *.pdb
cd TempPE
del Properties.Resources.Designer.cs.dll