namespace VkApi


type AccessDeniedException internal (error: InnerError) =
    inherit VkException (error)


