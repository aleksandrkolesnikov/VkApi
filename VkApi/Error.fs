namespace VkApi

open Newtonsoft.Json


[<Struct; NoEquality; NoComparison>]
type internal RequestParam [<JsonConstructor>] (key: string, value: string) =
    member _.Key = key
    member _.Value = value


[<Struct; NoEquality; NoComparison>]
type internal InnerError [<JsonConstructor>] (error_code: int, error_msg: string, request_params: RequestParam array) =
    member _.Code = error_code
    member _.Message = error_msg
    member _.Params = request_params


[<Struct; NoEquality; NoComparison>]
type internal Error [<JsonConstructor>] (error: InnerError) =
    member _.Error = error
