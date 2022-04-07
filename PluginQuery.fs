module PowerToys.Run.Plugin.RiderWorkspaces.PluginQuery

open System
open System.IO
open System.Xml.Linq

//let writeDebug txt =
//        let userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
//        let txt = txt |> sprintf "%A"
//        let outpath = @"%USERPROFILE%\debug.md" |> Environment.ExpandEnvironmentVariables
//        System.IO.File.AppendAllText(outpath,txt)

let riderSolutionsPath =
    @"%APPDATA%\JetBrains\"
    |> Environment.ExpandEnvironmentVariables
    |> Directory.EnumerateDirectories
    |> Seq.tryFind (fun f ->
        f |> Path.GetFileName
        |> (fun f -> f.StartsWith "Rider20")
    )
    |> Option.map (fun f -> Path.Combine(f, "options"))
    |> Option.map (fun f -> Path.Combine(f, "recentSolutions.xml"))
    
let recentSolutions =
        if riderSolutionsPath.IsNone
        then (failwith "Jetbrains Rider was not found in %APPDATA%\JetBrains")
        else
            riderSolutionsPath.Value
            |> (fun f ->
                let xml = File.ReadAllText f |> XElement.Parse
                let entries = 
                    xml.Descendants("entry")
                    |> Seq.toArray
                entries
            )

module Metadata =
    let (|ReplaceEnvVar|_|) (pattern:string) (str:string) =  
        if str.Contains(pattern)  
        then Some(fun replacement ->
            str.Replace(pattern, Environment.ExpandEnvironmentVariables(replacement)))  
        else None
    let expandEnvVar str = str |> Environment.ExpandEnvironmentVariables
    let replaceEnvVar var =
        match var with
        | ReplaceEnvVar "$USER_HOME$" fn -> fn "%USERPROFILE%"
        | containsvar when containsvar.Contains("$") -> failwithf "unknown variable in %A" containsvar
        | _ -> var
        
module RiderSolutionEntry =        
    let getSolutionFilePath (entry:XElement) =
        entry.Attribute("key").Value
        |> Metadata.replaceEnvVar
    let getMetaInfo (elem:XElement) = elem.Element("value").Element("RecentProjectMetaInfo")
    
module RiderSolutionMetaInfo =
    let title (metainfo:XElement) =
        metainfo.Attribute("frameTitle")
        |> Option.ofObj
        |> Option.map (fun f -> f.Value)
    let lastOpened (metainfo:XElement)  =
        metainfo.Elements("option")
        |> Seq.find (fun f -> f.Attribute("name").Value = "projectOpenTimestamp")
        |> (fun f -> f.Attribute("value").Value)


type SolutionInfoRecord =
    {
        title: string
        path: string
        lastOpened: DateTimeOffset
    }
    static member ofXmlEntry (entry:XElement) =
            let path = entry |> RiderSolutionEntry.getSolutionFilePath
            let metainfo = entry |> RiderSolutionEntry.getMetaInfo
            let title = metainfo |> RiderSolutionMetaInfo.title
            let lastOpened =
                metainfo |> RiderSolutionMetaInfo.lastOpened
                 |> Int64.TryParse
                 |> (fun (succ,value) ->
                     if succ then value|> DateTimeOffset.FromUnixTimeMilliseconds
                     else DateTimeOffset.Now )
                 
            {
                path = path
                title = title |> Option.defaultValue path
                lastOpened = lastOpened
            }
                
            