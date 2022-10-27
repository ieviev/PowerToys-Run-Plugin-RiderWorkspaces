
//exports from: ./lib
#r @"lib\Wox.Infrastructure.dll"
#r @"lib\Wox.Plugin.dll"
//
#r @"bin\Debug\net6.0-windows\PowerToys.Run.Plugin.RiderWorkspaces.dll"


open PowerToys.Run.Plugin.RiderWorkspaces
open PowerToys.Run.Plugin.RiderWorkspaces.PluginQuery
open System.Diagnostics

let recentSolutions = 
    PluginQuery.recentSolutions

        

let xmlentries = 
    recentSolutions 
    |> Seq.map SolutionInfoRecord.ofXmlEntry 
    |> Seq.toArray


do xmlentries |> Seq.iter (printfn "%A")


let str1 = """C:\Users\kast/Documents/GitHub/resharper-fsharp/ReSharper.FSharp/ReSharper.FSharp.sln"""
let str2 = str1.Replace("/","\\")

let p = new Process( StartInfo = ProcessStartInfo(str2,"",UseShellExecute=true))
p.Start()
true