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