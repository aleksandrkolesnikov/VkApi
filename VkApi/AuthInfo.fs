namespace VkApi

open Newtonsoft.Json


[<Struct; NoEquality; NoComparison>]
type internal AuthInfo [<JsonConstructor>] (access_token: string, user_id: uint64) =
    member _.AccessToken = access_token
    member _.UserId = user_id