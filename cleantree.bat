echo ALWAYS run me before commiting to the source tree.

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
del *.*

cd..
cd Release
del *.*