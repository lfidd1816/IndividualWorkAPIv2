namespace IndividualWorkAPI.Requests;

public record InitUploadRequest(
    string title,
    string description,
    long fileSize,
    string mimeType,
    bool privacyStatus
);