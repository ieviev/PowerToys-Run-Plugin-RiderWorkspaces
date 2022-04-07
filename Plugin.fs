namespace PowerToys.Run.Plugin.RiderWorkspaces

open System
open System.Diagnostics
open System.IO
open PowerToys.Run.Plugin.RiderWorkspaces.PluginQuery
open Wox.Plugin

type RiderPlugin() =
    let data =
        lazy 
            recentSolutions
            |> Seq.map SolutionInfoRecord.ofXmlEntry
            |> Seq.toArray
            
    let createResult name sub (act:ActionContext->bool) =
        Result(
            Action=Func<ActionContext,bool>act,
            SubTitle=sub,
            Title=name
        )
    
        
    interface IPlugin with
        member this.Description: string = "RiderWorkspaces"
        member this.Name: string = "RiderWorkspaces"
        member this.Init(context) = data.Force() |> ignore
        member this.Query(query) =
            
            
            data.Value
            |> Seq.choose (fun f ->
                let slnname = f.path |> Path.GetFileNameWithoutExtension
                
                let mutable score =
                    if slnname.Equals(query.Search,StringComparison.OrdinalIgnoreCase) then Int32.MaxValue
                    elif slnname.StartsWith(query.Search,StringComparison.OrdinalIgnoreCase) then (Int32.MaxValue / 2)
                    elif slnname.ToLower().Contains(query.Search.ToLower()) then (Int32.MaxValue / 4)
                    else 0
                if score = 0 then None else
                
                let dayspassed = DateTimeOffset.Now.Subtract(f.lastOpened).TotalHours |> int    
                score <- score - dayspassed
                
                let folder = f.path |> Path.GetDirectoryName
                let dateonly = f.lastOpened.ToString("yyyy-MM-dd")
                let subtitle = $"Project Folder: {folder}, Last opened {dateonly} "
                let res =
                    createResult slnname subtitle (fun d ->
                        ProcessStartInfo(f.path,"",UseShellExecute=true)
                        |> Process.Start
                        |> ignore
                        true
                    )
                res.Score <- score
                res.IcoPath <- "images\\rider.png"
                if res.Score = -1 then None else Some res
            )
            |> ResizeArray

            
                
                
                
                
                
