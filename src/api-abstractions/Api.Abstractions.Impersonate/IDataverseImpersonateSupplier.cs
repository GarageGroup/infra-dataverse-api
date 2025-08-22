using System;

namespace GarageGroup.Infra;

public interface IDataverseImpersonateSupplier<out T>
{
    [Obsolete("Use CallerObjectId input parameter instead.")]
    T Impersonate(Guid callerId);
}