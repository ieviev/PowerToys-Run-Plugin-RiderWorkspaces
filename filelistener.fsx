#load @"D:\git\scripts\commonimports.fsx" 
open System
open System.IO
open System.Text
open FSharp.Data
open FSharpPlus
open FSharpPlus.Data
open Fs
open Fs.Utils

open System.Threading
open System

let watcher = new System.IO.FileSystemWatcher(".","*.fs")
let sub fn = watcher.Changed.Subscribe(fun f -> fn f)
watcher.EnableRaisingEvents <- true
watcher.IncludeSubdirectories <- true

let mutable nextCall = DateTimeOffset.Now // limit to one per 10s

sub (fun f -> 
    if DateTimeOffset.Now < nextCall then () else
    nextCall <- nextCall.AddSeconds(10)
    printfn $"sub:%A{f.Name}"
    Process.createPowerShell "pwsh ./postbuild.ps1" |> ignore
)

let resetevent = new System.Threading.ManualResetEvent(false)
resetevent.WaitOne()