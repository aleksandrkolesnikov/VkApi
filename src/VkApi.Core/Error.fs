namespace VkApi.Core

open Newtonsoft.Json


type ErrorCode =
    | UnknownError = 1
    | TooManyRequestsPerSecond = 6


//[<Struct; NoEquality; NoComparison>]
type internal RequestParam [<JsonConstructor>] (key: string, value: string) =
    member _.Key = key
    member _.Value = value


//[<Struct; NoEquality; NoComparison>]
type internal InnerError [<JsonConstructor>] (error_code: int, error_msg: string, request_params: RequestParam array) =
    member _.Code = enum<ErrorCode> error_code
    member _.Message = error_msg
    member _.Params = request_params


//[<Struct; NoEquality; NoComparison>]
type internal ErrorWrapper [<JsonConstructor>] (error: InnerError) =
    member _.Error = error
