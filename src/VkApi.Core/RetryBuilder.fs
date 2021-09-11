namespace VkApi.Core

open System.Threading.Tasks
open FSharp.Control.Tasks.V2


type RetryBuilder (attempts) =
    let rec Retry n (f: unit -> Task<_>) (rest: 'T -> Task<_>) =
        task {
            try
                let! x = f ()
                let! y = rest x
                return y

            with error ->
                if n = 0 then
                    return raise (new System.Exception (""))
                else
                    do! Task.Delay 1000
                    return! Retry (n - 1) f rest
        }

    member _.Bind (f: unit -> Task<_>, rest: 'T -> Task<_>) =
        Retry (attempts - 1) f rest

    member _.Return x = task { return x }


[<AutoOpen>]
module RetryBuilderMod =
    let retry = RetryBuilder
