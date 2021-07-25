namespace VkApi.Core

open Newtonsoft.Json


type internal AuthenticationInfo [<JsonConstructor>] (access_token: string, user_id: uint64) =
    member _.AccessToken = access_token
    member _.UserId = user_id
