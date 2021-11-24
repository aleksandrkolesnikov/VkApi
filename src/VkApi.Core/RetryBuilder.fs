namespace VkApi.Core

open System.Threading.Tasks


type RetryBuilder (attempts) =
    let rec Retry n (f: unit -> Task<_>) =
        task {
            try
                return! f ()
            with
                | :? TooManyRequestsException as ex ->
                    if n = 0 then
                        return raise ex
                    else
                        do! Task.Delay 1000
                        return! Retry (n - 1) f
                | _ as ex ->
                    return raise ex
        }

    member _.Bind (f: unit -> Task<_>, _rest: 'T -> Task<_>) =
        Retry (attempts - 1) f

    member _.Return value = task { return value }

    member _.ReturnFrom (f: unit -> Task<_>) =
        Retry (attempts - 1) f
    
    member _.Zero () = task { () }


[<AutoOpen>]
module RetryBuilderMod =
    let retry = RetryBuilder
