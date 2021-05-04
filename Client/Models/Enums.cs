
namespace www.pwa.Client.Models {
    public enum UploadStatus
    {
        Uploading,
        UploadSuccess,
        UploadFailed,
        UploadDone
    }

    public enum RunStatus {
        Init,
        Running,
        Paused,
        FinishedAndTransmitting,
        FinishedAndTrasmitFailed,
        FinishedAndTransmitted,

    }
}