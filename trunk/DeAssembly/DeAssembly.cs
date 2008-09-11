using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//ProgramBlock
//ProgramLine
//equality simplifier
//expression simplifier
namespace DeAssembly
{
    class ProgramZ
    {
        static void Mainz(string[] args)
        {
            string filename = "2-29.asm";

            if (args.Length >= 1)
                filename = args[0];

            if (File.Exists(filename))
            {
                System.IO.StreamReader file = file = new System.IO.StreamReader(filename); ;
                LinkedList<string> code = new LinkedList<string>();
                string tempLine;

                while ((tempLine = file.ReadLine()) != null)
                    code.AddLast(tempLine);

                file.Close();

                //strip out all comments (makes parsing easier).
                code = UtilStripComments(code);

                LinkedList<string> decompiledFile = new LinkedList<string>();
                foreach (string line in code)
                    decompiledFile.AddLast(DecompileLine(line));

                Console.WriteLine("{");
                foreach (string line in decompiledFile)
                    Console.WriteLine(line);
                Console.WriteLine("}");
            }

            Console.WriteLine("\nFinished.");
            Console.ReadLine();
        }

        static string DecompileLine(string line)
        {
            string processedLine = "";
            string[] lineParameters = line.ToLower().Split(' ').Skip(1).ToArray();
            string command = line.ToLower().Split(' ')[0];

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
            if (line.Contains(':'))
                //processedLine = "Unhandled Label: \"" + line + "\"";
                processedLine = line;
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
                        processedLine = "Unidentified: \"" + line + "\"";
                        break;
                }

                //finalized
                processedLine += ";";
            }

            return processedLine;
        }

        static public LinkedList<string> UtilStripComments(LinkedList<string> code)
        {
            LinkedList<string> updatedList = new LinkedList<string>();

            foreach(string line in code)
            {
                if (line.Contains("#"))
                {
                    string processedLine = line.Substring(0, line.IndexOf('#'));
                    
                    //strip spaces
                    string[] lineTokens = processedLine.ToLower().Split(' ');
                    processedLine = "";
                    foreach (string token in lineTokens)
                        if (!token.Equals(""))
                            processedLine = processedLine + token + " ";

                    updatedList.AddLast(processedLine.Trim());
                }
                else
                    updatedList.AddLast(line.Trim());
            }

            return updatedList;
        }
    }
}
