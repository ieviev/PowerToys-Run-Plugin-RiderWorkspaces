open System
open System.Diagnostics

let watcher = new System.IO.FileSystemWatcher(".","*.fs")
let sub fn = watcher.Changed.Subscribe(fun f -> fn f)
watcher.EnableRaisingEvents <- true
watcher.IncludeSubdirectories <- true

let mutable nextCall = DateTimeOffset.Now // limit to one build per 10s

sub (fun f -> 
    if DateTimeOffset.Now < nextCall then () else
    nextCall <- nextCall.AddSeconds(10)
    //printfn $"sub:%A{f.Name}"
    let psstartinfo = ProcessStartInfo("powershell","./postbuild.ps1")
    use proc = new Process(StartInfo=psstartinfo)
    proc.Start() |> ignore
    proc.WaitForExit()
)

let resetevent = new System.Threading.ManualResetEvent(false)
resetevent.WaitOne()