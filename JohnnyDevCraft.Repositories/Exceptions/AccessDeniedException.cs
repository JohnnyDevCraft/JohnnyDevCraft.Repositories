using System;

namespace JohnnyDevCraft.Repositories.Exceptions;

public class AccessDeniedException : Exception
{
    public const string ErrorMessage = "You are not authorized to read this record.";

    public AccessDeniedException() : base(ErrorMessage)
    {
    }
}