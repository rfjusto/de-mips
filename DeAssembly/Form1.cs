using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DeAssembly
{
    public partial class Form1 : Form
    {
        public Form1(string[] args)
        {
            InitializeComponent();

            string filename = "2-29.asm";
            //filename = "Fibonacci.asm";

            if (args.Length >= 1)
                filename = args[0];

            if (File.Exists(filename))
                Decompile(filename);
            else
                MessageBox.Show("");
        }

        public void Decompile(string filename)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            LinkedList<ProgramLine> decompiledFile = new LinkedList<ProgramLine>();
            string tempLine;

            while ((tempLine = file.ReadLine()) != null)
                decompiledFile.AddLast(new ProgramLine(tempLine));

            file.Close();

            //strip out all comments (makes parsing easier).
            UtilStripComments(decompiledFile);

            string[] displayAsm = new string[decompiledFile.Count];
            string[] displayCode = new string[decompiledFile.Count];
            int i = 0;
            foreach(ProgramLine line in decompiledFile)
            {
                displayAsm[i] = line.Assembly;
                DecompileLine(line);
                displayCode[i] = line.CPP;
                i++;
            }

            TextBoxInput.Lines = displayAsm;

            Console.WriteLine("{");
            TextBoxOutput.Lines = displayCode;
            TextBoxOutput.Select(0,0); //HACK: Windows automatically selects the contents...?
            Console.WriteLine("}");
        }

        private void DecompileLine(ProgramLine line)
        {
            string processedLine = "";
            string[] lineParameters;

            if (!line.Assembly.Contains(":"))
                lineParameters = line.Assembly.Substring(line.Assembly.IndexOf(' ') + 1).ToLower().Split(' ');
            else
                lineParameters = new string[0];

            string command = line.Assembly.ToLower().Split(' ')[0];

            //strip out any tailing ',' characters or starting '$'
            for(int i = 0; i < lineParameters.Length; i ++)
            {
                lineParameters[i] = lineParameters[i].TrimEnd(',');
                lineParameters[i] = lineParameters[i].TrimStart('$');
            }

            //any $zero's will now be replaced.
            for (int i = 0; i < lineParameters.Length; i++)
                if (lineParameters[i].Equals("zero"))
                    lineParameters[i] = "0";

            //LABEL
            if (line.Assembly.Contains(":"))
                //processedLine = "Unhandled Label: \"" + line + "\"";
                processedLine = line.Assembly;
            //COMMAND
            else
            {
                switch (command.ToLower())
                {
                    case "addi":
                    case "add":
                        processedLine = MathParser.SimplifyEqual(lineParameters[0], MathParser.SimplifyArithmetic("+", lineParameters[1], lineParameters[2]));
                        break;

                    case "beq":
                        processedLine = "if ( " + lineParameters[0] + " == " + lineParameters[1] + " ) goto " + lineParameters[2];
                        break;

                    case "bne":
                        processedLine = "if ( " + lineParameters[0] + " != " + lineParameters[1] + " ) goto " + lineParameters[2];
                        break;

                    case "j":
                        processedLine = "goto " + lineParameters[0];
                        break;

                    case "subi":
                    case "sub":
                        processedLine = MathParser.SimplifyEqual(lineParameters[0], MathParser.SimplifyArithmetic("-", lineParameters[1], lineParameters[2]));
                        break;

                    case "sll":
                        processedLine = MathParser.SimplifyEqual(lineParameters[0], MathParser.SimplifyArithmetic("*", lineParameters[1], lineParameters[2]));
                        break;

                    default:
                        processedLine = "Unidentified: \"" + line.Assembly + "\"";
                        break;
                }

                //finalized TODO: move to C emitter
                processedLine += ";";
            }

            line.CPP = processedLine;
        }

        private void UtilStripComments(LinkedList<ProgramLine> code)
        {
            foreach (ProgramLine line in code)
            {
                string lineRemainder = "";

                if (line.Assembly.Contains("#"))
                {
                    line.Assembly = line.Assembly.Trim();
                    int z = line.Assembly.IndexOf('#');
                    lineRemainder = line.Assembly.Substring(0, z);

                    //strip spaces
                    string[] lineTokens = lineRemainder.ToLower().Split(' ');
                    lineRemainder = "";
                    foreach (string token in lineTokens)
                        if (!token.Equals(""))
                            lineRemainder = lineRemainder + token + " ";
                    
                }
                else
                {
                    lineRemainder = line.Assembly;
                }
                line.Assembly = lineRemainder.Trim(); // the above code will almost always leave a tailing space
            }
        }
    }
}
