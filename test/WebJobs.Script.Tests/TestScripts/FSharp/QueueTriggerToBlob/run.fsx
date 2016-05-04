open System
open System.Linq
open System.Threading.Tasks
open System.IO
open Microsoft.Azure.WebJobs.Host
open System.Runtime.InteropServices

type WorkItem() =
    member val Id = "" with get, set

let Run(input: WorkItem, [<Out>] output: byref<string>, log: TraceWriter) = 
    let json = String.Format("{{ \"id\": \"{0}\" }}", input.Id)    
    log.Info(sprintf "F# script processed queue message '%s'" json)
    output <- json