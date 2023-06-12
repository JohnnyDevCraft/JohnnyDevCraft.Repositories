using System;

namespace JohnnyDevCraft.Repositories.Exceptions;

public class TenantNotSpecifiedException : Exception
{
    public const string ErrorMessage = "Tenant is missing from user claims.";

    public TenantNotSpecifiedException() : base(ErrorMessage)
    {
    }
}