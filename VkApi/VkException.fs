namespace VkApi

open System


type VkException internal (error: InnerError) =
    inherit Exception (error.Message)

    let (method, args) =
        error.Params
        |> Array.partition (fun (param: RequestParam) -> param.Key = "method")
        |> fun (method, args) ->
            (method.[0].Value, args |> Array.map (fun arg -> $"{arg.Key}={arg.Value}"))

    member _.Code = error.Code
    member _.Method = method
    member _.Args = args
