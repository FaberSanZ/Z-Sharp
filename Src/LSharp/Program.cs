﻿using System;
using System.IO;
using System.Linq;
using LSharp.IL;


namespace LSharp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            bool showTree = true;
            string src = File.ReadAllText(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\Samples\Test.zk")));

            Console.Write(">>> ");
            Console.WriteLine(src);

            SyntaxTree syntaxTree = SyntaxTree.Parse(src);

            if (showTree)
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                PrettyPrint(syntaxTree.Root);
                Console.ForegroundColor = color;
            }
            Console.Write("output -> ");

            if (!syntaxTree.Diagnostics.Any())
            {
                Evaluator e = new Evaluator(syntaxTree.Root);
                int result = e.Evaluate();
                Console.WriteLine(result);
            }
            else
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkRed;

                foreach (string diagnostic in syntaxTree.Diagnostics)
                {
                    Console.WriteLine(diagnostic);
                }

                Console.ForegroundColor = color;
            }

            TestExe();

        }

        private static void TestExe()
        {
            string outputPath = Path.ChangeExtension(@"..\..\..\..\..\Samples\Test", ".exe");

            string moduleName = Path.GetFileNameWithoutExtension(outputPath);
            AssemblyNameDefinition assemblyName = new AssemblyNameDefinition(moduleName, new Version(1, 0));


            AssemblyDefinition _assemblyDefinition = AssemblyDefinition.CreateAssembly(assemblyName, moduleName, ModuleKind.Console);

            TypeDefinition typeDefinition = new TypeDefinition("", "Program", TypeAttributes.Abstract | TypeAttributes.Sealed);


            TypeReference tr = _assemblyDefinition.MainModule.ImportReference(typeof(void));

            MethodDefinition methodDefinition = new MethodDefinition("Main", MethodAttributes.Public, tr);
            typeDefinition.Methods.Add(methodDefinition);


            _assemblyDefinition.MainModule.Types.Add(typeDefinition);

            _assemblyDefinition.Save(outputPath);
        }


        private static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            string marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "   " : "│  ";

            SyntaxNode lastChild = node.GetChildren().LastOrDefault();

            foreach (SyntaxNode child in node.GetChildren())
            {
                PrettyPrint(child, indent, child == lastChild);
            }
        }


        
    }
}

