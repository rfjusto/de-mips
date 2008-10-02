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
// Michael Bradley                                                     //
//                                                                     //
/////////////////////////////////////////////////////////////////////////

#define ENABLE_V4300I_INSTRUCTIONS

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DeMIPS
{
    /// <summary>
    /// Handles GUI events and components.
    /// </summary>
    public partial class DeAssemblyForm : Form
    {
        #region constants

        private const string DEFAULT_FILENAME = "2-30.asm";
        //private const string DEFAULT_FILENAME = "AlleyCat.asm";

        #endregion

        #region variables

        OpenFileDialog fileSelectionDialog;
        DeAssembly myDecompiler;

        #endregion

        #region entry point
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DeAssemblyForm(args));
        }
        #endregion

        #region constructor

        /// <summary>
        /// Initializes GUI and starts decompilation process.
        /// </summary>
        /// <param name="args">If args[0] exists, it will automatically be decompiled.</param>
        public DeAssemblyForm(string[] args)
        {
            InitializeComponent();

            fileSelectionDialog = new OpenFileDialog();
            fileSelectionDialog.Filter = "mips assembly (*.asm)|*.asm|All files (*.*)|*.*";

            myDecompiler = new DeAssembly();

            if (args.Length >= 1)
                TextBoxFileName.Text = args[0];
            else
                TextBoxFileName.Text = DEFAULT_FILENAME;

            ButtonDecompile_Click(null, null);
        }

        #endregion

        #region GUI events

        private void ButtonSelectFile_Click(object sender, EventArgs e)
        {
            if (fileSelectionDialog.ShowDialog() == DialogResult.OK)
            {
                TextBoxFileName.Text = fileSelectionDialog.FileName;
                LabelSynced.Text = "Desynchronized";
            }
        }

        private void ButtonDecompile_Click(object sender, EventArgs e)
        {
            string filename = TextBoxFileName.Text;

            if (File.Exists(filename))
            {
                UtilDebugConsole.Flush();
                string[] assemblySource = myDecompiler.LoadAssemblyFile(filename);

                myDecompiler.DecompileAssembly(assemblySource, TextBoxOutput);

                TextBoxInput.Lines = assemblySource;
                LabelSynced.Text = "Synchronized";
                TextBoxConsole.Lines = UtilDebugConsole.GetAsArray();

                //TODO: make TextBoxConsole auto scroll down.
                TextBoxConsole.ScrollToCaret();
            }
            else
                MessageBox.Show("Cannot find: " + filename);
        }

        private void ButtonQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

    }
}
