open System
open Microsoft.Azure.WebJobs.Host

let Run(input: string, [<Out>] item: byref<string>, log: TraceWriter) =
    log.Info "F# ApiHub trigger function processed a file..."
    item <- input

