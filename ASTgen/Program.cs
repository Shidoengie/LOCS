﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LOCS;

namespace ASTgen
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: generate AST[script]");
                Environment.Exit(64);

            }
            string outpDir = args[0];
            string[] ast_list = {
                "Binary   : Expr left, Token op, Expr right",
                "Grouping : Expr expression",
                "Literal  : Object value",
                "Unary    : Token op, Expr right"
            };
            define_ast(outpDir, "Expr", ast_list.ToList());

        }
        private static void define_ast(string output_dir, string basename, List<string> types)
        {
            string path = $"{output_dir}/{basename}.cs";
            StreamWriter file = File.CreateText(path);
            file.WriteLine("using System;");
            file.WriteLine("using System.Collections.Generic;");
            file.WriteLine("using LOCS");
            file.WriteLine();
            file.WriteLine("abstract class " + basename);
            file.WriteLine("{");
            foreach (string type in types)
            {
                string classname = type.Split(":")[0].Trim();
                string fields = type.Split(":")[1].Trim();
                define_type(file, basename, classname, fields);
            }
            file.WriteLine("}");
            file.Flush();
        }
        private static void define_type(StreamWriter file,string basename,string classname,string fieldlist)
        {
            file.WriteLine("    sealed class "+classname+" : "+basename);
            file.WriteLine("    {");
            file.WriteLine($"       {classname}({fieldlist})");
            file.WriteLine("        {");
            string[] fields = fieldlist.Split(", ");
            foreach (string field in fields)
            {
                string name = field.Split(" ")[1];
                file.WriteLine($"          this.{name} = {name};");
            }
            file.WriteLine("        }");
            file.WriteLine();
            foreach (string field in fields)
            {
                file.WriteLine("    "+field + ";");
            }
            file.WriteLine("    }");
            
        }
        private void define_visitor(StreamWriter file, string baseName,List<string> types)
        {
            file.WriteLine("        public interface visitor");
            file.WriteLine("        {");
            foreach (var child in types)
            {
                var name = child.Split(":")[0].Trim();

                file.WriteLine($"        T Visit{name}{baseName}({baseName}.{name} {baseName.ToLower()});");
            }

            file.WriteLine("    }");
        }
    }
}
