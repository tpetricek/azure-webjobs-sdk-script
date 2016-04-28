//----------------------------------------------------------------------------------------
// This prelude allows scripts to be edited in Visual Studio or another F# editing environment 

#if !COMPILED
#I "../../bin/Binaries/WebJobs.Script.Host"
#r "Microsoft.Azure.WebJobs.Host.dll"
#r "Microsoft.Azure.WebJobs.Extensions.dll"
#endif

//----------------------------------------------------------------------------------------
// This is the implementation of the function 

open System
open System.IO
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Host

let Run(inputPath: string, output: byref<string>, log: TraceWriter ) =
    let input = File.ReadAllText(inputPath)
    let message = sprintf "F# script processed queue message '%s'" input
    output <- message


