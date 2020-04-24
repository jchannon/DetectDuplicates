open System.IO
open System.Linq
open System.Text.RegularExpressions

type FileReader = string -> Result<string seq,exn>

let output (data:seq<string*string>) =
    data 
    |> Seq.iter (fun (key,value) -> printfn "Duplicate %s found in %s" key value)

let getPath (args: string []) =
    match args.Any() with
    | true -> args.[0]
    | false -> Directory.GetCurrentDirectory()

let getFiles path =
    printfn "Looking for duplicates in %s" path
    Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories)

let readFile : FileReader =
    fun path ->
        try
            seq { use reader = new StreamReader(File.OpenRead(path))
                  while not reader.EndOfStream do
                      yield reader.ReadLine() }
            |> Ok
        with
        | ex -> Error ex
        
let parseLine (fileName:string) (line:string)  =
    let regexMatch = Regex.Match(line, "PackageReference Include=\"([^\"]*)\"")
    match regexMatch.Success with
    | true -> 
        Some (regexMatch.Groups.[1].Value, fileName)
    | _ -> None

let parse (fileName:string) (data:string seq) =
    data
    |> Seq.map (fun x-> parseLine fileName x)
    |> Seq.choose id
    
    
let findDuplicates (packages:seq<string * string>) =
    packages
    |> Seq.groupBy (fun s -> s)
    |> Seq.filter (fun (key, values) -> (values.Count() >= 2)) 
    |> Seq.map (fun(key,_)-> key)
    
let checkFile (file: string) =
    let fileName = Path.GetFileName(file)
    try
        match file |> readFile with
        | Ok data -> data |> parse fileName |> findDuplicates   
        | Error ex ->   printfn "Error: %A" ex.Message
                        Seq.empty<string*string>
    with
    | ex -> printfn "Error processing entry in %s" fileName
            Seq.empty<string*string>

let loopFiles (files: string []) =
    Array.map (fun x -> checkFile x) files

let processProjectFiles (args: string []) =
    args
    |> getPath
    |> getFiles
    |> loopFiles
    |> Seq.collect (fun x->x)

[<EntryPoint>]
let main argv =

    let dups = processProjectFiles argv
    dups |> output

    printfn "Duplicate detection complete"
    
    if dups.Any() then
        1
    else
        0