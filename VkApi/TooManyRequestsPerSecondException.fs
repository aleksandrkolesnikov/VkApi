namespace VkApi


type TooManyRequestsPerSecondException internal (error: InnerError) =
    inherit VkException (error)

