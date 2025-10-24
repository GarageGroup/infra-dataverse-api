namespace GarageGroup.Infra;

public enum DataverseFailureCode
{
    Unknown,

    Unauthorized,

    RecordNotFound,

    PicklistValueOutOfRange,

    UserNotEnabled,

    PrivilegeDenied,

    Throttling,

    SearchableEntityNotFound,

    DuplicateRecord,

    InvalidPayload,

    RecipientEmailNotFound,

    InvalidFileSize,

    IsvAborted,

    CannotUpdateBecauseItIsReadOnly
}