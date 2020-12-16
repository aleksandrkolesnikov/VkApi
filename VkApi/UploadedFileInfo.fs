namespace VkApi

open Newtonsoft.Json


type internal UploadedFileInfo =
    struct
        val Info: string

        [<JsonConstructor>]
        new file = { Info = file }

        member self.Title = self.Info.Split('|').[7]
    end