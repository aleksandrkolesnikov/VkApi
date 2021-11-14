namespace VkApi.Core

open System


type VkException (message) =
    inherit Exception (message)

type TooManyRequestsException (message) =
    inherit VkException (message)



[<RequireQualifiedAccess>]
module internal Exception =

    let RaiseSpecificException (error: InnerError) =
        match error.Code with
        | ErrorCode.UnknownError -> raise <| VkException ("Unknown error. Try to repeat later.")
        | ErrorCode.TooManyRequestsPerSecond -> raise <| TooManyRequestsException ("Too many requests per second.")
        | _ -> raise <| VkException (error.Message)
