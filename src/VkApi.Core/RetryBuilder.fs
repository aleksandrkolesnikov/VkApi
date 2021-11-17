namespace VkApi.Core

open System.Threading.Tasks


type RetryBuilder (attempts) =
    let rec Retry n (f: unit -> Task<_>) (rest: 'T -> Task<_>) =
        task {
            try
                let! x = f ()
                let! y = rest x
                return y

            with
                | :? TooManyRequestsException as ex ->
                    if n = 0 then
                        return raise ex
                    else
                        do! Task.Delay 1000
                        return! Retry (n - 1) f rest
                | _ as ex ->
                    return raise ex
        }

    member _.Bind (f: unit -> Task<_>, rest: 'T -> Task<_>) =
        Retry (attempts - 1) f rest

    member _.Return value = task { return value }
    
    member _.Zero () = task { () }


[<AutoOpen>]
module RetryBuilderMod =
    let retry = RetryBuilder
