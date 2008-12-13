/////////////////////////////////////////////////////////////////////////
//                                                                     //
//    DeMIPS - A MIPS decompiler                                       //
//                                                                     //
//        Copyright (c) 2008 by Ruben Acuna and Michael Bradley        //
//                                                                     //
// This file is part of DeMIPS.                                        //
//                                                                     //
// DeMIPS is free software; you can redistribute it and/or             //
// modify it under the terms of the GNU Lesser General Public          //
// License as published by the Free Software Foundation; either        //
// version 3 of the License, or (at your option) any later version.    //
//                                                                     //
// This library is distributed in the hope that it will be useful,     //
// but WITHOUT ANY WARRANTY; without even the implied warranty of      //
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       //
// GNU Lesser General Public License for more details.                 //
//                                                                     //
// You should have received a copy of the GNU Lesser General Public    //
// License along with this library; if not, write to the Free Software //
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA           //
// 02111-1307, USA, or contact the author(s):                          //
//                                                                     //
// Ruben Acuna <flyingfowlsoftware@earthlink.net>                      //
// Michael Bradley <mbradley1372@cox.net>                              //
//                                                                     //
/////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DeMIPS
{
    class DeAssembly
    {
        #region variables

        IFrontend activeFrontend;
        IBackend activeBackend;

        #endregion

        #region constuctor

        /// <summary>
        /// DeAssembly constructor. Setup Frontend and Backend.
        /// </summary>
        public DeAssembly()
        {
            activeFrontend = new FrontendMIPS();
            activeBackend = new BackendC();
        }

        #endregion

        #region methods

        /// <summary>
        /// Decompiles source code in a given array. The result is placed into the TextBox that was passed.
        /// </summary>
        /// <param name="assemblyFile">Array of strings containting code.</param>
        /// <param name="TextBoxOutput">TextBox that result will be copied to.</param>
        public void DecompileAssembly(string[] assemblyFile, TextBox TextBoxOutput) //HACK: there should be a better way.
        {
            //TODO: we need to identify blocks here and split them apart.
            ProgramBlock newBlock = new ProgramBlock();
            string[] sourceFile;

            activeFrontend.Preprocess(assemblyFile);
            
            foreach(string line in assemblyFile)
            {
                try
                {
                    newBlock.Program.AddLast(activeFrontend.TranslateLine(line, newBlock));
                }
                catch (Exception err)
                {
                    UtilDebugConsole.AddException(err);
                }
            }

            SimplifyStructures.SimplifyProgramBlock(newBlock);

            sourceFile = activeBackend.EmitBlock(newBlock);

            sourceFile = activeBackend.ProcessOutput(sourceFile);

            if(!newBlock.IsSane())
                UtilDebugConsole.AddException(new ExceptionWarning("DeAssembly - Insane Blocks were found."));

            if (assemblyFile.Length != sourceFile.Length)
                UtilDebugConsole.AddException(new ExceptionWarning("The number of input and output lines differ, GUI may function incorrectly."));

            //FIXME: this should really be in the gui code, however, if we pass the
            //textbox as a object it doesn't update. And we can't ref it either.
            TextBoxOutput.Lines = sourceFile;
        }

        #endregion

    }

}
