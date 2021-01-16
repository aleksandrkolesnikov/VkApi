namespace VkApi

open Newtonsoft.Json


[<NoEquality; NoComparison>]
type internal AuthInfo =
    struct
        val AccessToken: string
        val UserId: uint64

        [<JsonConstructor>]
        new (access_token, user_id) =
            {
                AccessToken = access_token
                UserId = user_id
            }
    end