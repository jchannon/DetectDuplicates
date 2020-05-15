using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DetectDuplicates
{
    class Program
    {
        static int Main(string[] args)
        {
            var path = string.Empty;

            path = args.Any() ? args[0] : Directory.GetCurrentDirectory();

            Console.WriteLine($"Looking for duplicates in {path}");
            Console.WriteLine("");

            bool duplicatesFound = false;

            var files = "*.csproj;*.fsproj".Split(';')
                .SelectMany(g => Directory.EnumerateFiles(path, g, SearchOption.AllDirectories));
            
            //var files = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var line = string.Empty;
                try
                {
                    var hash = new HashSet<string>();
                    using var fs = File.OpenRead(file);
                    using var sr = new StreamReader(fs);

                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine()?.Trim();
                        if (line == null)
                        {
                            continue;
                        }

                        if (!line.StartsWith(@"<PackageReference"))
                        {
                            continue;
                        }

                        //Regex to pull out package name from <PackageReference Include="Foo" . It will create a regex group within the double quotes of the package name
                        var match = Regex.Match(line, "Include=\"([^\"]*)\"");
                        if (!match.Success)
                        {
                            continue;
                        }

                        var name = match.Groups[1].Value;

                        //Add the name of current package line
                        if (hash.Add(name))
                        {
                            continue;
                        }

                        //If it fails to add to hash then it's because it already exists and therefore we have a duplicate
                        if (!duplicatesFound)
                        {
                            duplicatesFound = true;
                        }


                        Console.WriteLine($"Duplicate {name} found in {fileName}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error processing entry: {line} in {fileName}");
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Duplicate detection complete!");

            if (duplicatesFound)
            {
                return 1;
            }

            return 0;
        }
    }
}