namespace VkApi.Wrappers

open Newtonsoft.Json


type internal Response<'T> =
    struct
        val Response: 'T

        [<JsonConstructor>]
        new response = { Response = response }
    end