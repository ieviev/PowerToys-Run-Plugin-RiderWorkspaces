
//exports from: ./lib
#r @"lib\Wox.Infrastructure.dll"
#r @"lib\Wox.Plugin.dll"
//
#r @"bin\Debug\net6.0-windows\PowerToys.Run.Plugin.RiderWorkspaces.dll"


open PowerToys.Run.Plugin.RiderWorkspaces
open PowerToys.Run.Plugin.RiderWorkspaces.PluginQuery

let recentSolutions = 
    PluginQuery.recentSolutions

let xmlentries = 
    recentSolutions 
    |> Seq.map SolutionInfoRecord.ofXmlEntry 
    |> Seq.toArray

