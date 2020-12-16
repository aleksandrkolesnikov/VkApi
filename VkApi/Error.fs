namespace VkApi

open Newtonsoft.Json


type internal Error =
    struct
        val Error: InnerError

        [<JsonConstructor>]
        new error = { Error = error }
    end

and internal InnerError =
    struct
        val Code: int
        val Message: string
        val Params: RequestParam array

        [<JsonConstructor>]
        new (error_code, error_msg, request_params) =
            {
                Code = error_code
                Message = error_msg
                Params = request_params
            }
    end

and internal RequestParam =
    struct
        val Key: string
        val Value: string

        [<JsonConstructor>]
        new (key, value) =
            {
                Key = key
                Value = value
            }
    end