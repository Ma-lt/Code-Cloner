using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var code = loadFile(@"C:\Users\hp\Documents\Class1.cs");

            Random random = new Random();
            int cloneNumber = 1;
            int cloneRepetitions = 3;
            int methodIndex;
            int linesToClone;
            int l, h;
            l = 2;
            h = 4;

            int carry = 0;

            var sourceText = SourceText.From(code);
            //var sf = CSharpSyntaxTree.ParseText(sourceText);
            var newLines = sourceText.Lines.ToList();

            var syntaxTree = SyntaxFactory.ParseSyntaxTree(sourceText);
            var syntaxRoot = syntaxTree.GetRoot();
            List<MethodDeclarationSyntax> methodList = syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

            for (int i = 0; i < cloneNumber; i++)
            {
                methodIndex = random.Next(0, methodList.Count);
                var methodToClone = methodList.ElementAt(methodIndex);

                var methodBodySpan = methodToClone.Body.GetLocation().GetLineSpan();
                var methodStart = methodBodySpan.StartLinePosition.Line + 1;
                var methodEnd = methodBodySpan.EndLinePosition.Line - 1;

                Console.WriteLine("Del metodo: " + methodToClone.Identifier);

                var methodBodySize = methodEnd - methodStart;

                var cloneStart = 0;

                if (l >= methodBodySize)
                {
                    linesToClone = methodBodySize;
                    cloneStart = methodStart;
                }
                else
                {
                    linesToClone = random.Next(l, h+1);
                    cloneStart = random.Next(methodStart, methodEnd - linesToClone + 1);
                }

                carry += linesToClone;

                Console.WriteLine(linesToClone + " lineas a partir de la linea " + cloneStart);

                var minRIndex = methodList.Count;

                int offset = 0;

                for (int j = 0; j < cloneRepetitions; j++)
                {
                    var receivingMethodIndex = random.Next(0, methodList.Count);
                    if (receivingMethodIndex <= minRIndex)
                    {
                        minRIndex = receivingMethodIndex;
                        offset = 0;
                    }
                    else
                    {
                        offset = carry;
                    }
                    var receivingMethod = methodList.ElementAt(receivingMethodIndex);
                    var receivingMethodBodySpan = receivingMethod.Body.GetLocation().GetLineSpan();
                    var start = receivingMethodBodySpan.StartLinePosition.Line + 1;
                    var end = receivingMethodBodySpan.EndLinePosition.Line;
                    var insertPos = random.Next(start,end);
                    int cont = 1;

                    Console.WriteLine("En el metodo: " + receivingMethod.Identifier);
                    Console.WriteLine("A partir de la linea: " + insertPos + "\n");


                    for (int k = 0; k < linesToClone; k++)
                    {
                        newLines.Insert(insertPos + cont + offset, sourceText.Lines[cloneStart + k]);
                        cont++;
                    }
                }
               
                methodList.RemoveAt(methodIndex);

            }

            using (StreamWriter writer =
            new StreamWriter("clonedCode.txt"))
            {
                foreach (var m in newLines)
                {
                    writer.WriteLine(m.ToString());
                }
            }

            Console.ReadKey();
        }


        public static String loadFile(string path)
        {
            string readContents;
            using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
            {
                readContents = streamReader.ReadToEnd();
            }
            return readContents;
        }
    }
}
